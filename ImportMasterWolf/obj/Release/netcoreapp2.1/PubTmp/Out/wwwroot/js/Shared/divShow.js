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


function bindTableSearch1(inputId, tableClass) {
    const input = document.querySelector(`#${inputId}`);
    if (!input) return; // ป้องกันกรณี element ไม่มี

    input.addEventListener("keyup", () => {
        const filter = input.value.toLowerCase();
        const rows = document.querySelectorAll(`.${tableClass} tbody tr`);

        rows.forEach(row => {
            const cells = row.querySelectorAll("td");
            let match = false;

            cells.forEach(cell => {
                if (cell.textContent.toLowerCase().includes(filter)) {
                    match = true;
                }
            });

            row.style.display = match ? "" : "none";
        });
    });



}


function bindTableSearch(inputId, tableClass) {

    const input = document.getElementById(inputId);
    if (!input) return;

    function filterTable() {
        const filter = input.value.toLowerCase();
        const rows = document.querySelectorAll("." + tableClass + " tbody tr");

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(filter) ? "" : "none";
        });
    }

    // search ตอนพิมพ์
    input.addEventListener("keyup", filterTable);

    // ⭐ search ทันทีตอนโหลดหน้า ถ้ามีค่า
    if (input.value.trim() !== "") {
        filterTable();
        document.querySelectorAll(".accordion-content").forEach(x => {
            x.style.display = "block";
            //.style.display = "none";
        });

    }
}


bindTableSearch("searchInputTMasterData", "tbMTData");


$("#btnUploadFileMSTDATA").click(function () {
    $('#filesImport').val('');
    $("#myModalMSTDataUpload").modal("show");
});