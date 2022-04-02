$(document).ready(function ()
{
	NiceToMeetYou();
});

function WriteTraffic(controltype, control, value)
{
	$.netproxy("/webservice/traffic.asmx/TrafficBehaviour", { 'url': $(location).attr('href'), 'controltype': controltype, 'control': control, 'value': value });
}

function NiceToMeetYou()
{
	var w = window,
    d = document,
    e = d.documentElement,
    g = d.getElementsByTagName('body')[0],
    x = w.innerWidth || e.clientWidth || g.clientWidth,
    y = w.innerHeight || e.clientHeight || g.clientHeight;
	var sc = screen.width + 'x' + screen.height + ' ' + x + 'x' + y + ' ' + screen.colorDepth;
	$.netproxy("/webservice/traffic.asmx/NiceToMeetYou", { 'url': $(location).attr('href'), 'screen': sc }, function ()
	{
		if (!this.Error)
			$("#hplMySessionNr").text(this.SessionNr);
	});
}

function GetContent(textItem, container)
{
	$.netproxy("/Webservice/GetContent.asmx/GetJson", { "textItem": textItem }, function ()
	{
		$(container).append(this.Content);
	});
}

function GetReviews(name, container)
{
	$.netproxy("/Webservice/Reviews.asmx/GetJson", { "name": name }, function ()
	{
		var items = '';
		$.each(this, function (key, val)
		{
			items += '<div class="review mt30"><span class="score" title="' + DateTime2String(val.Datum) + ' | ' + val.Product + '">' + val.Score + '</span><span><b>' + val.Naam + '</b><br /></span><span>' + val.Toelichting + '</span></div>';
			if (key >= 2)
				return false;
		});
		$(container).html(items);
	});
}

