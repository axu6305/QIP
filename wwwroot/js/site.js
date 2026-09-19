(() => {
  const page = document.querySelector("[data-reference-page='true']");
  if (!page) {
    return;
  }

  const gridContainer = document.getElementById("patient-grid-container");
  const statusText = document.getElementById("grid-status");
  const referenceDateInput = document.getElementById("reference-date");
  const addForm = document.getElementById("patient-form");
  const editForm = document.getElementById("edit-patient-form");
  const refreshButton = document.getElementById("refresh-grid");
  const antiForgeryToken = document.getElementById("ajax-antiforgery")?.value;
  const editModalElement = document.getElementById("editPatientModal");
  const editModal = editModalElement ? new bootstrap.Modal(editModalElement) : null;

  const getReferenceDate = () => referenceDateInput.value;
  const commonHeaders = {
    "Content-Type": "application/json",
    "RequestVerificationToken": antiForgeryToken,
    "X-Requested-With": "XMLHttpRequest"
  };

  const setStatus = (message, isError = false) => {
    statusText.textContent = message;
    statusText.classList.toggle("text-danger", isError);
  };

  const syncReferenceDate = () => {
    const value = getReferenceDate();
    addForm.querySelector("[name='referenceDate']").value = value;
    editForm.querySelector("[name='referenceDate']").value = value;
  };

  const loadGrid = async () => {
    syncReferenceDate();
    setStatus("Loading…");

    const response = await fetch(`/Patients/Grid?referenceDate=${encodeURIComponent(getReferenceDate())}`, {
      headers: {
        "X-Requested-With": "XMLHttpRequest"
      }
    });

    if (response.status === 401) {
      window.location.href = "/Auth";
      return;
    }

    if (!response.ok) {
      gridContainer.innerHTML = "";
      setStatus("Unable to load patients.", true);
      return;
    }

    gridContainer.innerHTML = await response.text();
    const rowCount = gridContainer.querySelectorAll("tbody tr").length;
    setStatus(`${rowCount} patient${rowCount === 1 ? "" : "s"} shown`);
  };

  const submitPatient = async (form, successMessage) => {
    const payload = Object.fromEntries(new FormData(form).entries());
    payload.id = payload.id ? Number(payload.id) : null;
    payload.sex = Number(payload.sex);

    const response = await fetch("/Patients/Upsert", {
      method: "POST",
      headers: commonHeaders,
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const result = await response.json().catch(() => ({ message: "Unable to save patient." }));
      setStatus(result.message ?? "Unable to save patient.", true);
      return false;
    }

    setStatus(successMessage);
    await loadGrid();
    return true;
  };

  addForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const success = await submitPatient(addForm, "Patient added.");
    if (success) {
      addForm.reset();
      addForm.querySelector("[name='sex'][value='1']").checked = true;
      syncReferenceDate();
    }
  });

  editForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const success = await submitPatient(editForm, "Patient updated.");
    if (success && editModal) {
      editModal.hide();
    }
  });

  refreshButton.addEventListener("click", loadGrid);
  referenceDateInput.addEventListener("change", loadGrid);

  gridContainer.addEventListener("change", async (event) => {
    const checkbox = event.target.closest(".js-presence-toggle");
    if (!checkbox) {
      return;
    }

    const checked = checkbox.checked;
    const response = await fetch("/Patients/TogglePresence", {
      method: "POST",
      headers: commonHeaders,
      body: JSON.stringify({
        patientId: Number(checkbox.dataset.patientId),
        date: checkbox.dataset.date,
        isPresent: checked
      })
    });

    if (!response.ok) {
      checkbox.checked = !checked;
      setStatus("Unable to update ward day.", true);
      return;
    }

    setStatus("Ward day updated.");
    await loadGrid();
  });

  gridContainer.addEventListener("click", async (event) => {
    const editButton = event.target.closest(".js-edit-patient");
    if (editButton) {
      editForm.querySelector("[name='id']").value = editButton.dataset.id;
      editForm.querySelector("[name='surname']").value = editButton.dataset.surname;
      editForm.querySelector("[name='initials']").value = editButton.dataset.initials;
      editForm.querySelector("[name='hospitalNumber']").value = editButton.dataset.hospitalNumber;
      editForm.querySelector("[name='dateOfBirth']").value = editButton.dataset.dateOfBirthIso;
      editForm.querySelector(`[name='sex'][value='${editButton.dataset.sex}']`).checked = true;
      syncReferenceDate();
      editModal?.show();
      return;
    }

    const deleteButton = event.target.closest(".js-delete-patient");
    if (!deleteButton) {
      return;
    }

    if (!window.confirm("Delete this patient record?")) {
      return;
    }

    const response = await fetch("/Patients/Delete", {
      method: "POST",
      headers: commonHeaders,
      body: JSON.stringify({ patientId: Number(deleteButton.dataset.id) })
    });

    if (!response.ok) {
      setStatus("Unable to delete patient.", true);
      return;
    }

    setStatus("Patient deleted.");
    await loadGrid();
  });

  loadGrid();
})();
