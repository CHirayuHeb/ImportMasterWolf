document.querySelectorAll(".accordion-header").forEach(header => {
    header.addEventListener("click", () => {
        const content = header.nextElementSibling;
        const icon = header.querySelector(".toggle-icon");

        // toggle แสดง/ซ่อน
        if (content.style.display === "block") {
            content.style.display = "none";
            icon.textContent = "+";
        } else {
            content.style.display = "block";
            icon.textContent = "-";
        }
    });
});
