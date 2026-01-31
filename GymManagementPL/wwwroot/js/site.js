document.addEventListener("DOMContentLoaded", function () {
    const alertBox = document.getElementById("Alert");
    if (alertBox) {
        setTimeout(() => {
            alertBox.style.transition = "opacity 0.5s ease";
            alertBox.style.opacity = "0";
            setTimeout(() => alertBox.remove(), 500);
        }, 1500);
    }
});
document.getElementById('deleteModal')
    .addEventListener('show.bs.modal', function (e) {
        document.getElementById('deleteId').value =
            e.relatedTarget.getAttribute('data-id');
    });