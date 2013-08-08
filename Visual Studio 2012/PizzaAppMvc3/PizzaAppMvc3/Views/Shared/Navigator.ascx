<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) 
    {
%>
        <a class="brand">PizzaShop</a>
        <div class="nav-collapse">
            <ul class="nav">
                <li id="IndexActive"><%: Html.ActionLink("Home", "Index", "Home")%></li>
                <li id="OrdersActive"><%: Html.ActionLink( "Orders", "Orders", "Orders")%></li>
            </ul>
        </div>        
        <ul class="nav pull-right"> 
            <li id="ProfileActive"><%: Html.ActionLink("Profile", "Profile", "Account") %></li>                           
            <li id="SignOutActive"><%: Html.ActionLink("SignOut", "SignOut", "Account") %></li>
        </ul>       
<%
    }
    else 
    {
%> 
        <a class="brand">PizzaShop</a>
        <div class="nav-collapse">
            <ul class="nav">
                <li id="HomeActive"><%: Html.ActionLink("Home", "Index", "Home")%></li>
            </ul>
        </div>        
        <ul class="nav pull-right"> 
            <li id="SignInActive"><%: Html.ActionLink("SignIn", "SignIn", "Account") %></li>                           
            <li id="SignUpActive"><%: Html.ActionLink("SignUp", "SignUp", "Account") %></li>
        </ul>
<%
    }
%>