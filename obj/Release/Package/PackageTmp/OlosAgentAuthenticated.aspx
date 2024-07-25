<%@ Page Language="C#" Async="true" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="OlosAgentAuthenticated.aspx.cs" Inherits="OlosAgentSDK.OlosAgentAuthenticated" %>

<!DOCTYPE html>
<html lang="pt-br">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <!-- Jquery-3.7.1 -->
    <script src="Scripts/jquery-3.7.1.min.js"></script>

    <!-- Material Design Components JS -->
    <script src="public/js/materialize/materialize.min.js"></script>
    <script src="public/material-design/js/material-components-web.min.js"></script>
    <script src="public/material-design-snackbar/mdc.snackbar.min.js"></script>
    <script>mdc.autoInit();</script>

    <!-- Olos Agent SDK -->
    <script src="public/js/olosagentsdk.js"></script>
    <script src="public/js/handlers.js"></script>

    <!-- Material Design Components CSS -->
    <link href="public/material-design/material-components-web.min.css" rel="stylesheet">
    <link href="public/material-design/material-icons.css" rel="stylesheet">
    <link href="public/css/materialize/materialize.min.css" rel="stylesheet">
    <link href="public/material-design-snackbar/mdc.snackbar.min.css" rel="stylesheet" />
    <link href="public/css/styles.css" rel="stylesheet">

    <title>OLOS</title>
    <link rel="icon" type="image/x-icon" href="public/favicon.png" />
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>

        <header id="top-app-bar" class="mdc-top-app-bar">
            <div class="mdc-top-app-bar__row">
                <section class="mdc-top-app-bar__section mdc-top-app-bar__section--align-start">
                    <a href="#" class="brand-logo left mt-2 white-text mdc-top-app-bar__title">
                        <img src="public/olos-laranja.png" alt="Logo da OLOS" style="height: 45px;">
                    </a>
                </section>
                <section class="mdc-top-app-bar__section mdc-top-app-bar__section--align-end" role="toolbar">
                    <asp:UpdatePanel ID="updatePanelHangup" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <button id="btnHangup" class="mdc-button mdc-button--raised red darken-4 mr-2">
                                <span class="mdc-button__ripple"></span>
                                <span class="mdc-button__label">Desligar</span>
                                <i class="material-icons mdc-button__icon" aria-hidden="true">phone_disabled</i>
                            </button>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="updatePanelLogout" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <button class="mdc-button mdc-button--raised red darken-4" id="btnLogout">
                                <span class="mdc-button__ripple"></span>
                                <span class="mdc-button__label">Logout</span>
                                <i class="material-icons mdc-button__icon" aria-hidden="true">logout</i>
                            </button>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </section>
            </div>
        </header>

        <section style="padding-top: 100px;">
            <div class="container">
                <div class="row">
                    <div class="col s12">
                        <div class="card card-border blue darken-3">
                            <div class="card-content white-text">
                                <button id="changeStatusButton" class="mdc-button mdc-button--filled yellow darken-4" disabled="disabled">
                                    <p id="idChangeStatus" class="mdc-button__label"></p>
                                </button>

                                <hr class="custom-hr" />

                                <div class="centralizar-elementos">
                                    <button class="mdc-button mdc-button--filled blue darken-4" disabled="disabled">
                                        <p id="codigoAgentId"></p>
                                    </button>

                                    <button class="mdc-button mdc-button--filled blue darken-4" disabled="disabled">
                                        <p id="codigoAcessoLink"></p>
                                    </button>

                                    <button class="mdc-button mdc-button--filled blue darken-4" disabled="disabled">
                                        <p id="campaignIdAtiva"></p>
                                    </button>

                                    <button class="mdc-button mdc-button--filled blue darken-4" disabled="disabled">
                                        <p id="campaignIdReceptiva"></p>
                                    </button>

                                </div>
                            </div>
                            <div class="card-tabs">
                                <ul class="tabs tabs-fixed-width blue darken-4">
                                    <li class="tab"><a href="#ligacaoManual" class="active flow-text">Ligação manual</a></li>
                                    <li class="tab"><a href="#dispositons" class="flow-text">Dispositions</a></li>
                                    <li class="tab"><a href="#pausas" class="flow-text">Pausas</a></li>
                                </ul>
                            </div>
                            <div class="card-content">
                                <div id="ligacaoManual">
                                    <div class="row d-flex align-items-center">
                                        <div class="input-field filled-in col s3">
                                            <input id="inputDdd" class="blue darken-4" maxlength="2" placeholder=" ">
                                            <label for="inputDdd">DDD</label>
                                        </div>

                                        <div class="input-field filled-in col s6">
                                            <input id="inputPhoneNumber" class="blue darken-4" maxlength="9" placeholder=" ">
                                            <label for="inputPhoneNumber">Telefone</label>
                                        </div>

                                        <div class="input-field center-align">
                                            <button class="mdc-fab green darken-4" id="btnCallRequest" type="button" aria-label="phone">
                                                <span class="mdc-fab__ripple"></span>
                                                <i class="material-icons mdc-fab__icon">phone</i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                <div id="dispositons">
                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-2 container-gap">
                                        <asp:DropDownList ID="ddlDispositions" CssClass="browser-default blue darken-4 border-radius font-color-white mb-3" runat="server" />
                                    </div>

                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button id="btnThrowDisposition" class="mdc-button mdc-button--raised green darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Tabular</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">send</i>
                                        </button>

                                        <button id="btnSendDisposition" class="mdc-button mdc-button--raised blue darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Listar dispositions</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">list_alt</i>
                                        </button>
                                    </div>
                                </div>
                                <div id="pausas">
                                    <asp:DropDownList ID="ddlPausas" CssClass="browser-default blue darken-4 border-radius font-color-white mb-3" runat="server" />
                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button class="mdc-button mdc-button--raised green darken-4" id="btnPausa">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Enviar pausa</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">send</i>
                                        </button>

                                        <button class="mdc-button mdc-button--raised blue darken-4" id="btnRetornar" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Retornar pausa</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">flip_to_back</i>
                                        </button>

                                        <button class="mdc-button mdc-button--raised blue darken-4" id="btnListarReasons">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Listar pausas</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">list_alt</i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="snackbar" class="mdc-snackbar">
                <div class="mdc-snackbar__surface">
                    <div class="mdc-snackbar__label snackbar-font" role="status" aria-live="polite"></div>
                </div>
            </div>
        </section>
    </form>
</body>

</html>
