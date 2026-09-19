using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QIP.Web.Data;
using QIP.Web.Infrastructure;
using QIP.Web.Models;
using QIP.Web.ViewModels;

namespace QIP.Web.Controllers;

[WardAuthorize]
public class PatientsController(AppDbContext dbContext) : Controller
{
    private const int HistoryDays = 5;
    private const int FutureDays = 2;

    [HttpGet]
    public IActionResult Index()
    {
        return View(new WardDashboardViewModel
        {
            ReferenceDate = DateOnly.FromDateTime(DateTime.Today)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Grid(DateOnly? referenceDate)
    {
        var model = await BuildDashboardAsync(referenceDate ?? DateOnly.FromDateTime(DateTime.Today));
        return PartialView("_PatientGrid", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert([FromBody] PatientFormViewModel model)
    {
        if (!ModelState.IsValid || model.DateOfBirth is null || model.Sex is null || model.ReferenceDate is null)
        {
            return BadRequest(new { message = "Please complete the patient details." });
        }

        var normalizedHospitalNumber = model.HospitalNumber.Trim();
        var duplicateExists = await dbContext.Patients
            .AnyAsync(x => x.HospitalNumber.ToLower() == normalizedHospitalNumber.ToLower() && x.Id != model.Id.GetValueOrDefault());

        if (duplicateExists)
        {
            return Conflict(new { message = "Hospital folder number already exists." });
        }

        if (model.Id is int patientId)
        {
            var patient = await dbContext.Patients.FindAsync(patientId);
            if (patient is null)
            {
                return NotFound();
            }

            patient.Surname = model.Surname.Trim();
            patient.Initials = model.Initials.Trim();
            patient.HospitalNumber = normalizedHospitalNumber;
            patient.DateOfBirth = model.DateOfBirth.Value;
            patient.Sex = model.Sex.Value;
        }
        else
        {
            var patient = new Patient
            {
                Surname = model.Surname.Trim(),
                Initials = model.Initials.Trim(),
                HospitalNumber = normalizedHospitalNumber,
                DateOfBirth = model.DateOfBirth.Value,
                Sex = model.Sex.Value
            };

            dbContext.Patients.Add(patient);
            await dbContext.SaveChangesAsync();

            dbContext.WardPresences.Add(new WardPresence
            {
                PatientId = patient.Id,
                Date = model.ReferenceDate.Value
            });
        }

        await dbContext.SaveChangesAsync();
        return Ok(new { message = "Saved" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePresence([FromBody] TogglePresenceRequest request)
    {
        var patient = await dbContext.Patients.AnyAsync(x => x.Id == request.PatientId);
        if (!patient)
        {
            return NotFound();
        }

        var presence = await dbContext.WardPresences
            .FirstOrDefaultAsync(x => x.PatientId == request.PatientId && x.Date == request.Date);

        if (request.IsPresent)
        {
            if (presence is null)
            {
                dbContext.WardPresences.Add(new WardPresence
                {
                    PatientId = request.PatientId,
                    Date = request.Date
                });
            }
        }
        else if (presence is not null)
        {
            dbContext.WardPresences.Remove(presence);
        }

        await dbContext.SaveChangesAsync();
        return Ok(new { message = "Updated" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete([FromBody] DeletePatientRequest request)
    {
        var patient = await dbContext.Patients.FindAsync(request.PatientId);
        if (patient is null)
        {
            return NotFound();
        }

        dbContext.Patients.Remove(patient);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = "Deleted" });
    }

    private async Task<WardDashboardViewModel> BuildDashboardAsync(DateOnly referenceDate)
    {
        var startDate = referenceDate.AddDays(-HistoryDays);
        var endDate = referenceDate.AddDays(FutureDays);
        var dates = Enumerable.Range(0, HistoryDays + FutureDays + 1)
            .Select(offset => startDate.AddDays(offset))
            .ToList();

        var activePatientIds = await dbContext.Patients
            .Where(x => x.WardPresences.Any())
            .Select(x => new
            {
                x.Id,
                LastPresenceDate = x.WardPresences.Max(y => (DateOnly?)y.Date)
            })
            .Where(x => x.LastPresenceDate >= startDate)
            .Select(x => x.Id)
            .ToListAsync();

        var patients = await dbContext.Patients
            .Where(x => activePatientIds.Contains(x.Id))
            .Include(x => x.WardPresences)
            .OrderBy(x => x.Surname)
            .ThenBy(x => x.Initials)
            .ToListAsync();

        var rows = patients.Select(patient =>
        {
            var presenceDates = patient.WardPresences.Select(x => x.Date).ToHashSet();
            var totalWardDays = patient.WardPresences.Count;

            return new PatientRowViewModel
            {
                Id = patient.Id,
                Surname = patient.Surname,
                Initials = patient.Initials,
                HospitalNumber = patient.HospitalNumber,
                DobDisplay = patient.DateOfBirth.ToString("dd MMM yyyy"),
                DateOfBirthIso = patient.DateOfBirth.ToString("yyyy-MM-dd"),
                Sex = patient.Sex,
                TotalWardDays = totalWardDays,
                LastPresenceDate = patient.WardPresences.MaxBy(x => x.Date)?.Date,
                Days = dates.Select(day => new WardDayCellViewModel
                {
                    Date = day,
                    IsPresent = presenceDates.Contains(day),
                    IsCurrentDate = day == referenceDate
                }).ToList()
            };
        }).ToList();

        return new WardDashboardViewModel
        {
            ReferenceDate = referenceDate,
            Dates = dates,
            Patients = rows
        };
    }

    public sealed class TogglePresenceRequest
    {
        public int PatientId { get; set; }

        public DateOnly Date { get; set; }

        public bool IsPresent { get; set; }
    }

    public sealed class DeletePatientRequest
    {
        public int PatientId { get; set; }
    }
}
