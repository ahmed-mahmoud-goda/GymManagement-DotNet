
const connection = new signalR.HubConnectionBuilder()
                              .withUrl("/sessionHub")
                              .build();

connection.on("SessionStatusChanged", function (data) {
    console.log(data);
    showNotification(`Session: ${data.categoryName}`, `${data.categoryName} session<br>with Trainer: ${data.trainerName}<br>is now: ${data.status}<br>${data.timeRangeDisplay}`);
});

connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error:", err));


function showNotification(title, message) {
    const container = document.getElementById("toastContainer");

    const toast = document.createElement("div");
    toast.className = "toast text-bg-primary mb-2";
    toast.innerHTML = `
        <div class="toast-body bg-white text-dark d-flex justify-content-between align-items-start">
        <div>
            <h5 class="mb-1">${title}</h5>
            <div>${message}</div>
        </div>
        <button type="button" class="btn-close ms-2 mt-1" data-bs-theme="dark" onclick="this.closest('.toast').remove()"></button>
    </div>
    `;

    container.appendChild(toast);

    new bootstrap.Toast(toast, { delay: 5000 }).show();
}

function hideNotification() {
    const toast = document.getElementById("notification");
    toast.classList.remove("show");
    toast.classList.add("d-none");
}

