var btnPreviosS2 = document.querySelector("button.previos2");
var btnNextS2 = document.querySelector("button.next2");
var actionOTForm = "New\\OTForm";

if (btnPreviosS2 != null) {
    btnPreviosS2.addEventListener("click", function () {
        Back("2");
    });
}

if (btnNextS2 != null) {
    btnNextS2.addEventListener("click", async function () {
        let draft = await draftOTDocument();
        if (draft == "resolved") {
            await createNextstep("3");
            await GoToNextStep("3", actionOTForm);
        }
    });
}