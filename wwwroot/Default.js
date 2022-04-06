(function ()
{
	PageEvents();
	Init();
})();

var result;

function PageEvents()
{
	document.addEventListener("click", function (e)
	{
		if (typeof window[e.target.id] === "function")
			window[e.target.id].call(e, e);
	});

	$id("UploadExample").on("change", function (e)
	{
		StartUpload(e);
	});
}

function Init()
{
	result = $id('Result');
}

async function ShowBugs() //dont use this!
{
	r = await netproxyasync("./api/ComplexListOfStrings", {});
	r = await netproxyasync("./api/ComplexListOfInts", {});
	r = await netproxyasync("./api/ComplexArray", {});
}


var nnn;
var intNnn;

function C(s, a,b)
{
	if (nnn == s)
		intNnn++;
	else
	{
		nnn = s;
		intNnn = 1;
	}

	if (a != b)
		result.innerText += s + "(" + intNnn + ") :: " + a + ' != ' + b + '\n';
	return a != b;
}

async function UnitTest()
{
	result.innerText = '';
	var r;

	// The body name does not matter in the old valueproviders / binders
	//r = await netproxyasync("./api/ComplexTest/two?SomeParameter3=three",
	//	{
	//		Name: "My Name is",
	//		"Users":
	//			[
	//				[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
	//				[{ Name: "User10" }, { Name: "User11" }],
	//				[{ Name: "User20" }, { Name: "User21" }]
	//			]
	//	});
	r = await netproxyasync("./api/ComplexString", { Name: 'This is a test' });

	r = await netproxyasync("./api/ComplexTest2/two?SomeParameter3=three",
		{
			"SomeParameter4": // Now the beast has a name
			{
				Name: "My Name is",
				"Users":
					[
						[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
						[{ Name: "User10" }, { Name: "User11" }],
						[{ Name: "User20" }, { Name: "User21" }]
					]
			},
			"SomeParameter5" : "Yes you can" // double binder
		});

	r = await netproxyasync("./api/ComplexDouble", { F: 123.456 });
	C("ComplexDouble", r.F, 123.456);

	r = await netproxyasync("./api/ComplexDouble", { F: '123.456' }); // Needs JsonNumberHandling.AllowReadingFromString
	C("ComplexDouble", r.F, 123.456);

	r = await netproxyasync("./api/ComplexSingleObject", { AA: { a: 'aaa', b: 'bbb' } });
	C("ComplexSingleObject", r.AA.a, 'aaa');
	C("ComplexSingleObject", r.AA.b, 'bbb');

	r = await netproxyasync("./api/ComplexString", { Name: 'This is a test' });
	C("ComplexString", r.Name, 'This is a test');

	r = await netproxyasync("./api/ComplexStringInt", { Name: 'This is a test', A : 123 });
	C("ComplexStringInt", r.Name, 'This is a test');
	C("ComplexStringInt", r.A, 123);

	r = await netproxyasync("./api/ComplexListOfStrings", { ListOfStrings: ['a', '', 'b', null, 'c'] });
	C("ComplexList", r.ListOfStrings[0], 'a');
	C("ComplexList", r.ListOfStrings[1], '');
	C("ComplexList", r.ListOfStrings[2], 'b');
	C("ComplexList", r.ListOfStrings[3], null);
	C("ComplexList", r.ListOfStrings[4], 'c');

	r = await netproxyasync("./api/ComplexListOfInts", { ListOfInts: [ 0, 1 , 2, null , 4] });
	C("ComplexListInt", r.ListOfInts[0], 0);
	C("ComplexListInt", r.ListOfInts[1], 1);
	C("ComplexListInt", r.ListOfInts[2], 2);
	C("ComplexListInt", r.ListOfInts[3], null);
	C("ComplexListInt", r.ListOfInts[4], 4);
	
	r = await netproxyasync("./api/ComplexArray", { list: [{ a: 'a', b: null }, null, { a: 'c', b: 'd' }] });
	C("ComplexArray", r.list.length, 3);
	if (r.list.length == 3)
	{
		var r0 = r.list[0];
		var r1 = r.list[1];
		var r2 = r.list[2];
		C("ComplexArray", r0.a, 'a')
		C("ComplexArray", r0.b, null);
		C("ComplexArray", r1, null);
		C("ComplexArray", r2.a, 'c');
		C("ComplexArray", r2.b, 'd');
	}

	r = await netproxyasync("./api/ComplexArrayArray",
	{
		Group: "groupy",
		List:
		{
			Name: "My Name is",
			"Users":
				[
					["admins", "0"],
					["editors", "1"],
					null,
					["sisters", "3"]
				]
		}
	});
	C("ComplexArrayArray", r.Group, "groupy");
	var l = r.List;
	C("ComplexArrayArray", l.Name, 'My Name is');
	var uu = l.Users;
	C("ComplexArrayArray", uu.length, 4);
	var u = uu[0];
	C("ComplexArrayArray", u[0], 'admins');
	C("ComplexArrayArray", u[1], '0');
	u = uu[1];
	C("ComplexArrayArray", u[0], 'editors');
	C("ComplexArrayArray", u[1], '1');
	u = uu[2];
	C("ComplexArrayArray", u, null);
	if (uu.length == 4)
	{
		u = uu[3];
		C("ComplexArrayArray", u[0], 'sisters');
		C("ComplexArrayArray", u[1], '3');
	}

	r = await netproxyasync("./api/ComplexArrayArrayClass",
		{
			Testing: true,
			Relaxed: false,
			Group: "Nice Group",
			GroupInfo:
			{
				Name: "My Name is",
				"Users":
					[
						[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
						[{ Name: "User10" }, { Name: "User11" }],
						[{ Name: "User20" }, { Name: "User21" }]
					]
			}
		});
	C("ComplexArrayArrayClass", r.Testing, true);
	C("ComplexArrayArrayClass", r.Relaxed, false);
	C("ComplexArrayArrayClass", r.Group, "Nice Group");
	var i = r.GroupInfo;
	C("ComplexArrayArrayClass", i.Name, "My Name is");
	var us = i.Users;
	C("ComplexArrayArrayClass", us.length, 3);
	C("ComplexArrayArrayClass", us[0][0].Name, "User00");
	C("ComplexArrayArrayClass", us[0][0].Alias.length, 3);
	C("ComplexArrayArrayClass", us[0][1].Name, "User01");
	C("ComplexArrayArrayClass", us[0][1].Alias, null);
	C("ComplexArrayArrayClass", us[1][0].Name, "User10");
	C("ComplexArrayArrayClass", us[1][0].Alias, null);
	C("ComplexArrayArrayClass", us[1][1].Name, "User11");
	C("ComplexArrayArrayClass", us[1][1].Alias, null);
	C("ComplexArrayArrayClass", us[2][0].Name, "User20");
	C("ComplexArrayArrayClass", us[2][0].Alias, null);
	C("ComplexArrayArrayClass", us[2][1].Name, "User21");
	C("ComplexArrayArrayClass", us[2][1].Alias, null);
	var al = us[0][0].Alias;
	C("ComplexArrayArrayClass", al[0], 'aliasa');
	C("ComplexArrayArrayClass", al[1], 'aliasb');
	C("ComplexArrayArrayClass", al[2], 'aliasc');

	// Checking, run till end
	C("ready", true, false);
}


	//netproxy("./api/ComplexDouble", { F: null });
	//netproxy("./api/ComplexDouble", { F: 123.456 });
	//netproxy("./api/ComplexString", { Name: null });
	//netproxy("./api/ComplexString", { Name: 12.34 });
	//netproxy("./api/ComplexString", { Name: 'abc def' });
	//netproxy("./api/ComplexStringInt", { Name: 'abc def', A : 123 });
	//netproxy("./api/ComplexList", { list: [ "a", "b" , "", null, "d"] });
	//netproxy("./api/ComplexList", { list: ["a", "", "b", null, "c"] }); // "a" null "b" "b" "c" WTF?
	//netproxy("./api/ComplexListNullableDouble", { list: [1.2, 3.4, 5.6, null, 7.8] });
	//netproxy("./api/ComplexListObjecs", { list: [1.2, null, 3 ] }); // numbers to string, using CultureInfo!
	//netproxy("./api/ComplexStringList", { Name: 'abc def', list: [ 'a', 'b' , 'c'] });
	//netproxy("./api/ComplexObjectArray", { objB: { Name: 'Alphons', List: [{ a: 'a', b: 'b' }, { a: 'c', b: 'd' }] } });



function ProgressHandler(event)
{
	var percent = 0;
	var position = event.loaded || event.position;
	var total = event.total;
	if (event.lengthComputable)
		percent = Math.ceil(position / total * 100);
	$id("Result").innerText = "Uploading " + percent + "%";
}

function StartUpload(e)
{
	var file = e.target.files[0];

	$id("Result").innerText = 'Uploading';

	var formData = new FormData();

 	formData.append("file", file, file.name);
	formData.append("Form1", "Value1"); // some extra Form data

	netproxy("/api/Upload", formData, function ()
	{
		$id("Result").innerText = 'Ready Length:' + this.Length + " ExtraValue:" + this.Form1;
	}, window.NetProxyErrorHandler, ProgressHandler);
}
