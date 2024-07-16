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
    <script>mdc.autoInit();</script>

    <!-- Olos Agent SDK -->
    <script src="public/js/olosagentsdk.js"></script>
    <script src="public/js/handlers.js"></script>

    <!-- Material Design Components CSS -->
    <link href="public/material-design/material-components-web.min.css" rel="stylesheet">
    <link href="public/material-design/material-icons.css" rel="stylesheet">
    <link href="public/css/materialize/materialize.min.css" rel="stylesheet">
    <link href="public/css/styles.css" rel="stylesheet">

    <title>Integração Olos</title>
    <link rel="icon" type="image/x-icon" href="public/favicon.png" />
</head>

<body class="white darken-3">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>

        <header id="top-app-bar" class="mdc-top-app-bar">
            <div class="mdc-top-app-bar__row">
                <section class="mdc-top-app-bar__section mdc-top-app-bar__section--align-start">
                    <a href="#" class="brand-logo left ml-5 white-text mdc-top-app-bar__title">Integração Olos</a>
                </section>
                <section class="mdc-top-app-bar__section" role="toolbar">
                </section>
                <section class="mdc-top-app-bar__section mdc-top-app-bar__section--align-end" role="toolbar">
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
                                <span class="card-title">Interações com o Discador</span>

                                <button runat="server" id="btnHangup" class="mdc-button mdc-button--raised red darken-4" onserverclick="btnHangup_ServerClick">
                                    <span class="mdc-button__ripple"></span>
                                    <span class="mdc-button__label">Desligar Chamada</span>
                                    <i class="material-icons mdc-button__icon" aria-hidden="true">phone_disabled</i>
                                </button>

                                <hr class="custom-hr" />
                                <div class="centralizar-elementos">
                                    <p id="codigoAgentId" class="mr-3"></p>
                                    <p id="codigoAcessoLink" class="mr-3"></p>
                                    <p id="campaignIdAtiva"></p>
                                    <p id="campaignIdReceptiva"></p>
                                </div>
                            </div>
                            <div class="card-tabs">
                                <ul class="tabs tabs-fixed-width blue darken-4">
                                    <li class="tab"><a href="#pausas" class="active flow-text">Pausas</a></li>
                                    <li class="tab"><a href="#ligacaoManual" class="flow-text">Ligação manual</a></li>
                                    <li class="tab"><a href="#dispositons" class="flow-text">Dispositions</a></li>
                                </ul>
                            </div>
                            <div class="card-content">
                                <div id="pausas">
                                    <asp:DropDownList ID="ddlPausas" CssClass="browser-default blue darken-4 border-radius font-color-white mb-3" runat="server" />
                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button class="mdc-button mdc-button--raised green darken-4" id="btnPausa">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Confirmar</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">pause</i>
                                        </button>

                                        <button class="mdc-button mdc-button--raised yellow darken-4" id="btnRetornar" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Retornar</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">stop</i>
                                        </button>
                                    </div>
                                </div>
                                <div id="ligacaoManual">
                                    <div class="row">
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
                                    <asp:HiddenField ID="hfSelectedDisposition" runat="server" />

                                    <asp:DropDownList ID="ddlDispositions" CssClass="browser-default blue darken-4 border-radius font-color-white mb-3" runat="server" />

                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button id="btnLancarTabulacao" class="mdc-button mdc-button--raised green darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Lançar tabulação</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">check</i>
                                        </button>

                                        <button class="mdc-button mdc-button--raised yellow darken-4" id="btnListarDispositions" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Listar Dispositions</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">list</i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </form>
</body>

</html>
