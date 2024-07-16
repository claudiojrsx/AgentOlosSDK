document.addEventListener('DOMContentLoaded', function () {
    var tabs = document.querySelectorAll('.tabs');
    M.Tabs.init(tabs);
});

document.addEventListener("DOMContentLoaded", () => {
    function addButtonEventListener(buttonId, handlerFunctionName) {
        const button = document.getElementById(buttonId);
        if (button) {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                if (
                    typeof OlosAgent !== "undefined" &&
                    typeof OlosAgent[handlerFunctionName] === "function"
                ) {
                    if (handlerFunctionName === "agentLogout") {
                        displayCountdownToast();
                        OlosAgent[handlerFunctionName]();
                    } else {
                        OlosAgent[handlerFunctionName]();
                    }
                } else {
                    console.error(`OlosAgent.${handlerFunctionName} não está definida.`);
                }
            });
        } else {
            console.error(`Botão com id ${buttonId} não encontrado.`);
        }
    }

    function displayCountdownToast() {
        let countdown = 3;

        const updateToastMessage = () => {
            M.toast({
                text: `Fechando em ${countdown} segundos...`,
                displayLength: 1000,
                classes: 'btn',
                completeCallback: function () {
                    if (countdown === 0) {
                        if (window.opener != null) {
                            window.close();
                        } else {
                            setTimeout(() => {
                                M.toast({
                                    text: 'Esta janela não pode ser fechada automaticamente.',
                                    displayLength: 2000,
                                    classes: 'btn'
                                });
                            }, 3000);
                        }
                    }
                }
            });
        };

        const countdownInterval = setInterval(() => {
            if (countdown >= 0) {
                updateToastMessage();
                countdown--;
            } else {
                clearInterval(countdownInterval);
            }
        }, 2000);
    }

    addButtonEventListener("btnLogout", "agentLogout");
    addButtonEventListener("btnPausa", "agentReasonRequest");
    addButtonEventListener("btnRetornar", "agentIdleRequest");
});