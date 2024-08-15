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

    <title>Integração Olos</title>
    <link rel="icon" type="image/x-icon" href="public/favicon.png" />
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>

        <header id="top-app-bar" class="mdc-top-app-bar blue darken-4">
            <div class="mdc-top-app-bar__row menu-info">
                <section class="mdc-top-app-bar__section mdc-top-app-bar__section--align-start">
                    <a href="#" class="brand-logo left mt-2 white-text mdc-top-app-bar__title">
                        <img src="public/logo/cob-olos.png" alt="Logo da OLOS" style="height: 60px;">
                    </a>
                </section>
                <section class="mdc-top-app-bar__section mdc-top-app-bar__section--align-end" role="toolbar">
                    <button class="mdc-button mdc-button--filled blue darken-2 mr-3" type="button" disabled="disabled">
                        <span class="mdc-button__ripple"></span>
                        <span class="mdc-button__label">
                            <asp:Label ID="lblActiveConn" runat="server"></asp:Label>
                        </span>
                        <i class="material-icons mdc-button__icon" aria-hidden="true">sensors</i>
                    </button>

                    <asp:UpdatePanel ID="updatePanelHangup" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <button id="btnHangup" class="mdc-button mdc-button--raised red darken-4 mr-2" type="button">
                                <span class="mdc-button__ripple"></span>
                                <span class="mdc-button__label">Desligar</span>
                                <i class="material-icons mdc-button__icon" aria-hidden="true">phone_disabled</i>
                            </button>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:UpdatePanel ID="updatePanelLogout" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <button class="mdc-button mdc-button--raised red darken-4" id="btnLogout" type="button">
                                <span class="mdc-button__ripple"></span>
                                <span class="mdc-button__label">Logout</span>
                                <i class="material-icons mdc-button__icon" aria-hidden="true">logout</i>
                            </button>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </section>
            </div>
        </header>

        <section class="pt-6">
            <div class="container pb-6 pt-6 pr-5 pl-5 container-100">
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
                                    <li class="tab"><a href="#pausas" class="active flow-text">Pausas</a></li>
                                    <li class="tab"><a href="#dispositons" class="flow-text">Tabulação</a></li>
                                    <li class="tab"><a href="#ligacaoManual" class="flow-text">Ligação manual</a></li>
                                </ul>
                            </div>
                            <div class="card-content">
                                <div id="pausas">
                                    <asp:DropDownList ID="ddlPausas" CssClass="browser-default blue darken-4 border-radius font-color-white mb-3" runat="server" />
                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button class="mdc-button mdc-button--raised blue darken-4" id="btnListarReasons">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Listar pausas</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">list_alt</i>
                                        </button>

                                        <button class="mdc-button mdc-button--raised blue darken-4" id="btnRetornar" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Retornar pausa</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">flip_to_back</i>
                                        </button>

                                        <button class="mdc-button mdc-button--raised green darken-4" id="btnPausa" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Enviar pausa</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">send</i>
                                        </button>
                                    </div>
                                </div>
                                <div id="dispositons">
                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-2 container-gap">
                                        <asp:DropDownList ID="ddlDispositions" CssClass="browser-default blue darken-4 border-radius font-color-white mb-3" runat="server" />
                                    </div>

                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button id="btnSendDisposition" class="mdc-button mdc-button--raised blue darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Listar tabulações</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">list_alt</i>
                                        </button>

                                        <button id="btnThrowDisposition" class="mdc-button mdc-button--raised green darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Tabular</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">send</i>
                                        </button>
                                    </div>
                                </div>
                                <div id="ligacaoManual">

                                    <div class="mdc-layout-grid__cell mdc-layout-grid__cell--span-12 container-gap">
                                        <button id="btnManualCallState" class="mdc-button mdc-button--raised blue darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Modo Manual</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">dialpad</i>
                                        </button>

                                        <button id="btnManualCallEnd" class="mdc-button mdc-button--raised blue darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Encerrar modo manual</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">flip_to_back</i>
                                        </button>
                                    </div>

                                    <hr class="custom-hr" />

                                    <div class="row d-flex pb-3">
                                        <div class="input-field filled-in col s3">
                                            <input id="inputDdd" class="blue darken-4" maxlength="2" placeholder=" ">
                                            <label for="inputDdd">DDD</label>
                                        </div>

                                        <div class="input-field filled-in col s6">
                                            <input id="inputPhoneNumber" class="blue darken-4" maxlength="9" placeholder=" ">
                                            <label for="inputPhoneNumber">Telefone</label>
                                        </div>
                                    </div>

                                    <div class="d-flex">
                                        <button id="btnCallRequest" class="mdc-button mdc-button--raised green darken-4" type="button">
                                            <span class="mdc-button__ripple"></span>
                                            <span class="mdc-button__label">Discar</span>
                                            <i class="material-icons mdc-button__icon" aria-hidden="true">phone</i>
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

        <div id="divReturnEmail" class="lightboxdivModalPhone col-lg-12" hidden="hidden">
            <asp:UpdatePanel runat="server" ID="updReturnEmail" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField ID="ClickedLinkId" runat="server" />
        
                    <div class="panel panel-white">
                        <div class="panel-heading">
                            <div>
                                <ul class="nav nav-tabs padding-10" role="tablist">
                                    <li role="presentation" class="active">
                                        <a href="#UpdEmails" id="UpdEmails1" class="nome_aba-dash font-size-aba" role="tab" data-toggle="tab" onclick="storeLinkId(this.id)">Atualização cadastral</a>
                                    </li>
                                    <li role="presentation">
                                        <a href="#InsertEmails" id="InsertEmails1" class="nome_aba-dash font-size-aba" role="tab" data-toggle="tab" onclick="storeLinkId(this.id)">Inserir novo e-mail</a>
                                    </li>
                                </ul>
                            </div>
        
                            <a id="aReturnAtuEmail"
                                onclick="this.style.display='none';placeholdervalida();this.style.display='block';window.parent.chamaaguarde();"
                                onserverclick="aReturnAtuEmail_Click"
                                style="margin: -25px 60px;"
                                class="botao-aceitar"
                                runat="server"></a>
        
                            <a onclick="FechaBox(GetDadClass(this,'lightboxdiv'));"
                                style="margin: -25px 0;"
                                class="botao-resetar"></a>
                        </div>
        
                        <div role="tabpanel" class="tab-pane active fade in" id="UpdEmails">
                            <div class="panel-body">
                                <div class="padding-up-b">
                                    <asp:Label ID="lblReturnEmails" CssClass="label-weights" Text="Atualização cadastral (Return) - E-mails" runat="server" />
                                </div>
        
                                <asp:GridView ID="gdvReturnEmail" CssClass="tabela_nova display table-striped table-bordered dt-responsive responsive nowrap" CellSpacing="0" Width="100%" EmptyDataText="Não existe e-mail cadastrado para esse cliente." AutoGenerateColumns="false" runat="server">
                                    <Columns>
                                        <asp:BoundField DataField="ContactID" HeaderText="N Cliente" />
                                        <asp:BoundField DataField="EmailID" HeaderText="Id Email" />
                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                        <asp:BoundField DataField="EmailStatus" HeaderText="Status Atual" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlReturnStatusEmail" runat="server">
                                                    <asp:ListItem Text="Status: " Value="-1"></asp:ListItem>
                                                    <asp:ListItem Text="Incorreto" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Não definido" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Correto" Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="Verificado" Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Primário">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlReturnPrimarioEmail" runat="server">
                                                    <asp:ListItem Text="Primário: " Value="-1"></asp:ListItem>
                                                    <asp:ListItem Text="Sim" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Não" Value="0"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <%-- Fim do panel-body de atualização de e-mails --%>
        
                        <div role="tabpanel" class="tab-pane fade in" id="InsertEmails">
                            <div class="panel-body" visible="false">
                                <div class="padding-up-b">
                                    <asp:Label ID="lblReturnInsertEmails" CssClass="label-weights" Text="Inserir novo e-mail" runat="server" />
                                </div>
        
                                <asp:Label ID="lblReturnInsertEmail" CssClass="label-weights" Text="Digite o e-mail" AssociatedControlID="txtReturnInsertEmail" runat="server" />
                                <asp:TextBox ID="txtReturnInsertEmail" CssClass="form-control" Placeholder=" Digite o e-mail" runat="server" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>

</html>
