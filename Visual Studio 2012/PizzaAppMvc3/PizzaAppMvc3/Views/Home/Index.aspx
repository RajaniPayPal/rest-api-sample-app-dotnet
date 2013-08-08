<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../../Styles/StyleValidation.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #IndexActive,
        #HomeActive
        {
            color: #555555;
            text-decoration: none;
            box-shadow: inset 0px 3px 8px rgba(0,0,0,0.125);
            background-color: rgb(229, 229, 229);
        }  
    </style>
    <div class="container" id="content">        
        <div id="divAlertMessage" runat="server" visible="false">
            <button class="close" data-dismiss="alert" id="buttonAlertMessage" runat="server">
                ×</button>
            <label id="labelAlertMessage" runat="server">
            </label>
        </div>
        <div class="row pizza-row">
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 0" src="http://www.gravatar.com/avatar/0000000000000000000000000000000?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+0"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 1" src="http://www.gravatar.com/avatar/0000000000000000000000000000001?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+1"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 2" src="http://www.gravatar.com/avatar/0000000000000000000000000000002?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+2"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 3" src="http://www.gravatar.com/avatar/0000000000000000000000000000003?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+3"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 4" src="http://www.gravatar.com/avatar/0000000000000000000000000000004?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+4"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 5" src="http://www.gravatar.com/avatar/0000000000000000000000000000005?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+5"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
        </div>
        <div class="row pizza-row">
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 6" src="http://www.gravatar.com/avatar/0000000000000000000000000000006?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+6"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 7" src="http://www.gravatar.com/avatar/0000000000000000000000000000007?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+7"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 8" src="http://www.gravatar.com/avatar/0000000000000000000000000000008?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+8"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 9" src="http://www.gravatar.com/avatar/0000000000000000000000000000009?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+9"
                        class="btn btn-small" data-disable-with="Procesing.."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 10" src="http://www.gravatar.com/avatar/00000000000000000000000000000010?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+10"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 11" src="http://www.gravatar.com/avatar/00000000000000000000000000000011?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+11"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
        </div>
        <div class="row pizza-row">
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 12" src="http://www.gravatar.com/avatar/00000000000000000000000000000012?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+12"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 13" src="http://www.gravatar.com/avatar/00000000000000000000000000000013?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+13"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 14" src="http://www.gravatar.com/avatar/00000000000000000000000000000014?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+14"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 15" src="http://www.gravatar.com/avatar/00000000000000000000000000000015?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+15"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 16" src="http://www.gravatar.com/avatar/00000000000000000000000000000016?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+16"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
            <div class="span2">
                <div class="image">
                    <img alt="Pizza 17" src="http://www.gravatar.com/avatar/00000000000000000000000000000017?d=identicon&amp;f=y" />
                </div>
                <div class="details">
                    20$ - <a href="../PlaceOrders/PlaceOrders?order%5Bamount%5D=20&amp;order%5Bdescription%5D=Pizza+17"
                        class="btn btn-small" data-disable-with="Procesing..."
                        data-method="post" rel="nofollow">Buy</a>
                </div>
            </div>
        </div>
	    <br/>
        <br />
	    <div class="row">
		    <div class="span6 offset3">
		    <p>This is a sample application which showcases the new PayPal REST APIs. The App uses mock data to demonstrate how you can use the REST APIs for the following operations:</p>
		    <ul>
			    <li>Saving credit card information with PayPal for later use</li>
			    <li>Making payments using a saved credit card</li>
			    <li>Making payments using PayPal</li>
		    </ul>
		    </div>
	    </div>
    </div>
</asp:Content>
