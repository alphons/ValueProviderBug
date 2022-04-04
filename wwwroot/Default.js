﻿(function ()
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
}

function Init()
{
	result = $id('Result');
}

function NetproxyAsync(url, data)
{
	return new Promise((resolve, reject) =>
	{
		netproxy(url, data, resolve, reject );
	});
}

function C(a,b)
{
	if (a != b)
		result.innerText += a + ' != ' + b + '\n';
	return a != b;
}

async function UnitTest()
{
	result.innerText = '';
	var r;

	r = await NetproxyAsync("./api/ComplexDouble", { F: 123.456 });
	C(r.F, 123.456);

	r = await NetproxyAsync("./api/ComplexDouble", { F: '123.456' }); // JsonNumberHandling.AllowReadingFromString
	C(r.F, 123.456);

	r = await NetproxyAsync("./api/ComplexSingleObject", { AA: { a: 'aaa', b: 'bbb' } });

	r = await NetproxyAsync("./api/ComplexString", { Name: 'This is a test' });
	C(r.Name, 'This is a test');

	r = await NetproxyAsync("./api/ComplexStringInt", { Name: 'This is a test', A : 123 });
	C(r.Name, 'This is a test');
	C(r.A, 123);

	r = await NetproxyAsync("./api/ComplexList", { list: ['a', '', 'b', null, 'c'] });
	C(r.list[0], 'a');
	C(r.list[1], '');
	C(r.list[2], 'b');
	C(r.list[3], null);
	C(r.list[4], 'c');

	r = await NetproxyAsync("./api/ComplexListInt", { list: [ 0, 1 , 2, null , 4] });
	C(r.list[0], 0);
	C(r.list[1], 1);
	C(r.list[2], 2);
	C(r.list[3], null);
	C(r.list[4], 4);

	r = await NetproxyAsync("./api/ComplexArray", { list: [{ a: 'a', b: null }, null, { a: 'c', b: 'd' }] } );
	var r0 = r.list[0];
	var r1 = r.list[1];
	var r2 = r.list[2];
	C(r0.a, 'a')
	C(r0.b, null);
	C(r1, null);
	C(r2.a, 'c');
	C(r2.b, 'd');

	r = await NetproxyAsync("./api/ComplexArrayArray",
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
	C(r.Group, "groupy");
	var l = r.List;
	C(l.Name, 'My Name is');
	var uu = l.Users;
	C(uu.length, 4);
	var u = uu[0];
	C(u[0], 'admins');
	C(u[1], '0');
	u = uu[1];
	C(u[0], 'editors');
	C(u[1], '1');
	u = uu[2];
	C(u, null);
	u = uu[3];
	C(u[0], 'sisters');
	C(u[1], '3');

	r = await NetproxyAsync("./api/ComplexArrayArrayClass",
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
	C(r.Testing, true);
	C(r.Relaxed, false);
	C(r.Group, "Nice Group");
	var i = r.GroupInfo;
	C(i.Name, "My Name is");
	var us = i.Users;
	C(us.length, 3);
	C(us[0][0].Name, "User00");
	C(us[0][0].Alias.length, 3);
	C(us[0][1].Name, "User01");
	C(us[0][1].Alias, null);
	C(us[1][0].Name, "User10");
	C(us[1][0].Alias, null);
	C(us[1][1].Name, "User11");
	C(us[1][1].Alias, null);
	C(us[2][0].Name, "User20");
	C(us[2][0].Alias, null);
	C(us[2][1].Name, "User21");
	C(us[2][1].Alias, null);
	var al = us[0][0].Alias;
	C(al[0], 'aliasa');
	C(al[1], 'aliasb');
	C(al[2], 'aliasc');

	// Checking, run till end
	C(true, false);
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



