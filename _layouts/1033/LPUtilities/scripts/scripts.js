/// This contains common JavaScript functions used to open windows etc.</p>


	//String constants
	var L_UnableToEstablishVoiceConntection = "Voice connection cannot be established. \n Chat will attempt to be launched and you may be able to start a voice conversation from there.";
	var L_UnableToEstablishVideoConnection = "Video connection cannot be established \n Chat will attempt to be launched and you may be able to start a video conversation from there.";
	var L_HTMLRefreshWindow = "window will refresh  If the window does not refresh automatically <a href='javascript:location.reload;false;'>click here</a>";
	var L_HTMLUnableToGetBuddyList = "<span> <font color=red face=arial size=2>Unable to retrieve your Messenger buddy list. Make sure you are <a href=\"#\" onclick=\"signIn();\">signed-in</a>.</font></span>";
	var L_UnableToEstablishChatConnection = "Chat connection cannot be established";
	var messengerImageLocation="/_layouts/images/custom/";
	var agt=navigator.userAgent.toLowerCase(); 
	var is_ie   = (agt.indexOf("msie") != -1); 

	var thetable='';
            
    // Instant Messenger statuses
    function getStatus(state)
    {
        switch (state)
        {
            case 1: return "Offline";
            case 2: return "Online";
            case 6: return "Invisible";
            case 10: return "Busy";
            case 14: return "Be Right Back";
            case 18: return "Idle";
            case 34: return "Away";
            case 50: return "On the Phone";
            case 66: return "Out to Lunch";
        }

        return "Unknown";
    }

	/// Search Handler used by the Extended Search Web Control
    function searchHandler(path)
	{
	
		if(document.getElementById("txtScope").selectedIndex==document.getElementById("txtScope").options.length-1)
		{
		    //in case, user select the current site, open in the same window
			window.location=document.getElementById("txtScope").options[document.getElementById("txtScope").selectedIndex].value+document.getElementById("SearchString").value;
		}
		else
		{
		    //open in a new pop up window that is lanched on the center of the screen
			launchstring=document.getElementById("txtScope").options[document.getElementById("txtScope").selectedIndex].value+document.getElementById("SearchString").value;
			newwindow=launchCenter(launchstring,"Search","520","800","status=yes,toolbar=no,menubar=yes,location=no,scrollbars=yes,resizable=yes");
		}
	}
    
    /// Functions for Instant Messenger for users without Office 2003 integration
    function signIn()
        {
            if ("undefined" != typeof(document.all.msgrUI))
            {
                document.all.msgrUI.Signin(0,'','');
                window.setTimeout(location.reload, 5000);
                document.all.msgrList.innerHTML = L_HTMLRefreshWindow;
            }
        }

    function writeListError()
    {
        document.all.msgrList.innerHTML = L_HTMLUnableToGetBuddyList;
    }
        
    function startvoice(theuser)
    {     
		if ("undefined" != typeof(document.all.msgrUI))
		{
			try
			{
				msgrUI.StartVoice (theuser);
			}
			catch(err)	
			{
				alert(L_UnableToEstablishVoiceConntection);
				startmessage(theuser);
			}
		}
	}

    function startvideo(theuser)
    {
		if ("undefined" != typeof(document.all.msgrUI))
		{
			try
			{
				document.all.msgrUI.StartVideo (theuser);
			}
			catch(err)	
			{
				alert(L_UnableToEstablishVideoConnection);
				startmessage(theuser);
			}
		}
    }

    function startmessage(theuser)
    {
		if ("undefined" != typeof(document.all.msgrUI))   
		{
			try
			{
				document.all.msgrUI.InstantMessage (theuser);
			}
			catch(err)	
			{
				alert(L_UnableToEstablishChatConnection);
			}
		}
    }

    function addcontact(theuser)
    {
		if ("undefined" != typeof(document.all.msgrUI))
		{
			try
			{
				document.all.msgrUI.AddContact(0,theuser);
				//location.reload()
			}
			catch(exception)	
			{
				alert('Unable to add contact');
			}
		}
    }
        
    function sendmail(theuser)//need .net account
    {
		if ("undefined" != typeof(document.all.msgrUI))        
			document.all.msgrUI.SendMail (theuser)
    }
        
    function custstatus(theuser)
    {
        if ("undefined" != typeof(document.all.msgrUI))  
        {
			try 
			{
				var User= document.all.msgrUI.GetContact(theuser , document.all.msgr.MyServiceId);
				userstatus= (User.Status);
			}
			catch (exception)
			{
				userstatus=0;
			}
		}
		else
		{
			userstatus=-1;
		}
		return userstatus;
    }
        
        
    function ProcessContacts()
    {    
        var thetr=thetable.getElementsByTagName("TR");
        var thediv=thetable.getElementsByTagName("DIV");
       
        var thedivLEN=thediv.length;
        
        for( i = 0; i <thedivLEN; i++)
        {
			var UserEmail=thediv[i].innerHTML;
			var UserStatus=custstatus(UserEmail);
			var strVideo="<a href='javascript:startvideo(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"video.gif' border='0' alt='Video conference'></a>";
			var strAudio="<a href='javascript:startvoice(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"phone.gif' border='0' alt='talk'></a>";
			var strMessage="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"chat.gif' border='0' alt='Message'></a>";
			var strAddContact="<a href='javascript:addcontact(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"add.gif' border='0' alt='Add'></a>";
			
			switch (UserStatus)
            {
				case 0:
				
				thediv[i].innerHTML= strAddContact;
				thediv[i].style.display="inline";
				break;
                case 1: 
                //"Offline"
                thediv[i].innerHTML="<img src='"+messengerImageLocation+"offline.gif' border='0' alt='Offline'>";
				thediv[i].style.display="inline";
                break;
                case 2: 
                //"Online"
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"online.gif' border='0' alt='Message'></a>" + " " +strVideo + " " + strAudio;
				thediv[i].style.display="inline";
                break;
                case 6:
                //"Invisible"
                thediv[i].innerHTML="Invisible";
				thediv[i].style.display="inline";
                break;
                case 10: 
                //"Busy"
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"busy.gif' border='0' alt='Busy'></a>";
				thediv[i].style.display="inline";
                break;
                case 14: 
                //"Be Right Back"
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"beback.gif' border='0' alt='Be right back'></a>";
				thediv[i].style.display="inline";
                break;
                case 18: 
                //"Idle";
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"away.gif' border='0' alt='Idle'>";
				thediv[i].style.display="inline";
				break
                case 34: 
                //"Away"
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"away.gif' border='0' alt='Away'></a>";
				thediv[i].style.display="inline";
                break;
                case 50: 
                //"On the Phone"
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"onPhone.gif' border='0' alt='On the Phone'></a>";
				thediv[i].style.display="inline";
                break;
                case 66: 
                //"Out to Lunch"
                thediv[i].innerHTML="<a href='javascript:startmessage(\""+UserEmail+"\")'><img src='"+messengerImageLocation+"outto.gif' border='0' alt='Out to lunch'></a>";
				thediv[i].style.display="inline";
                break;
            }
  
		}
		}
		
	function InsertObject()
	{
		//MsgrObj = new ActiveXObject("Messenger.MsgrObject");
		//MsgrApp = new ActiveXObject("Messenger.MessengerApp");
		//divMsgrObject.innerHTML = "<OBJECT classid='clsid:F3A614DC-ABE0-11d2-A441-00C04F795683' codeType='application/x-oleobject' height='1' id='MsgrObj' width='1'></OBJECT><OBJECT classid='clsid:FB7199AB-79BF-11d2-8D94-0000F875C541' codeType='application/x-oleobject' height='1' id='MsgrApp' width='1'></OBJECT>";
		//divMsgrObject.innerHTML ="<object classid='clsid:F3A614DC-ABE0-11d2-A441-00C04F795683' id='msgr' width='1' height='1' style='display: none;' /><object classid='clsid:B69003B3-C55E-4B48-836C-BC5946FC3B28' id='msgrUI' width='1' height='1' style='display: none;' />"
	}
	
	// Open a URL in a new window
	function launchCenter(url, name, height, width,extraprops) {
	var str = "height=" + height + ",innerHeight=" + height;
	str += ",width=" + width + ",innerWidth=" + width;
	if (window.screen) {
		var ah = screen.availHeight - 30;
		var aw = screen.availWidth - 10;
		var xc = (aw - width) / 2;
		var yc = (ah - height) / 2;
		str += ",left=" + xc + ",screenX=" + xc;
		str += ",top=" + yc + ",screenY=" + yc+","+extraprops;
	}
	//added for my planner web part with Teacher Client and XP SP2
	if(url.charAt(url.length-1)=="/")
		url=url.substr(0,url.length-1);
	newwindow=window.open(url, name, str);
	newwindow.focus();
	return newwindow;
	}
	// Open a URL in an ew window 
	function launchCenter2(url, name,extraprops) {
		var width;
		var height;
		width=700;
		height=500;
		if(window.screen)
		{
			width=screen.availWidth * 0.8;
			height=screen.availHeight * 0.8;
		}

		var str = "height=" + height + ",innerHeight=" + height;
		str += ",width=" + width + ",innerWidth=" + width;
		if (window.screen) {
			var ah = screen.availHeight - 30;
			var aw = screen.availWidth - 10;
			var xc = (aw - width) / 2;
			var yc = (ah - height) / 2;
			str += ",left=" + xc + ",screenX=" + xc;
			str += ",top=" + yc + ",screenY=" + yc+","+extraprops;
		}
		//added for my planner web part with Teacher Client and XP SP2
		if(url.charAt(url.length-1)=="/")
			url=url.substr(0,url.length-1);
		newwindow=window.open(url, name, str);
		newwindow.focus();
		return newwindow;
	}

	// Function to display a day in a new window for the Calendar
	function ShowDay(title, eventsHtml)
	{
		var newWinJS;
		var newWinCss;
		
		newWinCss = "<LINK href=\"/_layouts/1033/lgutilities/styles/CustomStyles.css\" type=\"text/css\" rel=\"stylesheet\"><LINK href=\"/_layouts/1033/styles/sps.css\" type=\"text/css\" rel=\"stylesheet\">";
		newWinJS = "<script language=\"javascript\" src=\"/_layouts/images/scripts.js\">";
		newWindow=launchCenter('', 'DayView', '500',  '700', 'status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes');
		newWindow.document.open();
		newWindow.document.write ("<html><head>" + newWinCss + newWinJS + "<\/script><title>" + title + "</title></head><body>" + eventsHtml + "&nbsp;&nbsp;&nbsp;&nbsp;<input type='button' name='close' value='Close' onClick='window.close();'></body></html>");
		newWindow.document.close();
		return;
	}

	// Javascript for the MyTeams web part to open and close nodes of the tree
	function myTeamstoggle(x,i)
	{
		var node = document.getElementById(x);
		var img = document.getElementById(i);
		if (node.className == "MyTeamsDiv")
		{
			node.className = "MyTeamsTop";
			img.src = "/_layouts/images/minus.gif";
		}
		else
		{
			node.className = "MyTeamsDiv";
			img.src = "/_layouts/images/plus.gif";
		}
		return;
	}
	
	
	
	 function selectSharePointCalendar ( calendarID  ) 
      { 
         var selectedCalendarStr = this.document.getElementById ("selectedCalendars").value;
         if ( selectedCalendarStr.length == 0)  
             this.document.getElementById ( "selectedCalendars").value += calendarID;
             else 
             { 
                var idList  = this.document.getElementById ( "selectedCalendars").value.split (",");  
                  for (  i=0; i<idList.length;i++) 
                { 
                    if ( idList [i] == calendarID) 
                    return;
                }
                    this.document.getElementById ( "selectedCalendars").value += ',' + calendarID;
                }
               // alert ( this.document.getElementById ( "selectedCalendars").value);
             }
     
    
    function unselectSharePointCalendar ( calendarID)
            { //alert (calendarID); 
            var selectedCalendarStr = this.document.getElementById ( "selectedCalendars").value; 
             if ( selectedCalendarStr.length == 0) {return;}
             else { 	var idList  = this.document.getElementById ( "selectedCalendars").value.split (","); 
             for (  i=0; i<idList.length;i++) 	
            {	if ( idList [i] == calendarID)	idList[i]=0;	} this.document.getElementById ( "selectedCalendars").value = idList[0];	
            for ( j=1;j<idList.length;j++)	this.document.getElementById ( "selectedCalendars").value	 +=','+ idList[j];}
            //alert ( this.document.getElementById ( "selectedCalendars").value);
            }     
            
            
            
        function selectExchangeCalendar ( calendarID  ) 
            { 
             var selectedCalendarStr = this.document.getElementById ( "selectedCalendarsE").value;
             if ( selectedCalendarStr.length == 0)  this.document.getElementById ( "selectedCalendarsE").value += calendarID;
             else { var idList  = this.document.getElementById ( "selectedCalendarsE").value.split (",");  
            for (  i=0; i<idList.length;i++) 
            { if ( idList [i] == calendarID) return;}
            this.document.getElementById ( "selectedCalendarsE").value += ',' + calendarID;}
            
            //alert ( this.document.getElementById ( "selectedCalendarsE").value);
             }
            
            
           function unselectExchangeCalendar ( calendarID) 
           {
           //alert (calendarID); 
            var selectedCalendarStr = this.document.getElementById ( "selectedCalendarsE").value; 
             if ( selectedCalendarStr.length == 0) {return;}
            else { 	var idList  = this.document.getElementById ( "selectedCalendarsE").value.split (","); 
             for (  i=0; i<idList.length;i++) 	
            {	if ( idList [i] == calendarID)	idList[i]=0;	} this.document.getElementById ( "selectedCalendarsE").value = idList[0];	
            for ( j=1;j<idList.length;j++)	this.document.getElementById ( "selectedCalendarsE").value	 +=','+ idList[j];}
            //alert ( this.document.getElementById ( "selectedCalendarsE").value);
            }
            
            
            function ToggleShowExchange ( flag, ctrlId) 
            { 
            //alert ( ctrlId +","+ flag );
            var currentFlag = this.document.getElementById ( 'showExchange').value;
            if ( currentFlag == 0 )
            {            
             this.document.getElementById ( 'showExchange').value = 1;
            //alert (  this.document.getElementById (ctrlId).className); 
             this.document.getElementById (ctrlId).className = "btn_dn" ;
            } 
            else
            {            
             this.document.getElementById ( 'showExchange').value = 0;
             this.document.getElementById (ctrlId).className = "btn_up" ;
            } 
            }
              
            function ToggleShowSLK ( flag,ctrlId) 
            { 
                 var currentFlag = this.document.getElementById ( 'showslk').value;
                if ( currentFlag == 0 )
                    {            
                        this.document.getElementById ( 'showslk').value = 1;
                        this.document.getElementById (ctrlId).className = "btn_dn" ;
                    } 
            else
                   {            
                        this.document.getElementById ( 'showslk').value = 0;
                        this.document.getElementById (ctrlId).className = "btn_up" ;
                    }                 
              }