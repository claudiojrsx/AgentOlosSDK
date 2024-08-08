document.addEventListener('DOMContentLoaded', function () {
    var tabs = document.querySelectorAll('.tabs');
    M.Tabs.init(tabs);
});

function showSnackbar(message) {
    const snackbar = new mdc.snackbar.MDCSnackbar(document.querySelector('.mdc-snackbar'));
    snackbar.labelText = message;
    snackbar.open();
}

document.addEventListener("DOMContentLoaded", () => {
    function addButtonEventListener(buttonId, handlerFunctionName, callback) {
        const button = document.getElementById(buttonId);
        if (button) {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                if (
                    typeof OlosAgent !== "undefined" &&
                    typeof OlosAgent[handlerFunctionName] === "function"
                ) {
                    OlosAgent[handlerFunctionName]();
                    if (callback) {
                        callback();
                    }
                } else {
                    console.error(`OlosAgent.${handlerFunctionName} não está definida.`);
                }
            });
        } else {
            console.error(`Botão com id ${buttonId} não encontrado.`);
        }
    }

    addButtonEventListener("btnLogout", "agentLogout", () => {
        showSnackbar("Logout realizado com sucesso. A janela será fechada em 3 segundos.");
        setTimeout(() => {
            window.close();
        }, 3000);
    });
    addButtonEventListener("btnRetornar", "agentIdleRequest");
    addButtonEventListener("btnHangup", "hangupRequest");
    addButtonEventListener("btnSendDisposition", "listDispositions");
    addButtonEventListener("btnListarReasons", "listReasons");
    addButtonEventListener("btnManualCallState", "manualCallMode");
    addButtonEventListener("btnManualCallEnd", "endManualCallStateRequest");
});
