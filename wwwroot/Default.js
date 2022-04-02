(function ()
{
	PageEvents();
})();

function PageEvents()
{
	$id("Complex").on("click", function (e)
	{
		DoComplex();
	});
}

function DoComplex()
{
	var a = { Name: 'abc def', list: ['a', 'b', 'c'] };

	var b = JSON.stringify(a);


	//netproxy("./api/ComplexDouble", { F: null });
	//netproxy("./api/ComplexDouble", { F: 123.456 });
	//netproxy("./api/ComplexString", { Name: null });
	netproxy("./api/ComplexString", { Name: 12.34 });
	//netproxy("./api/ComplexString", { Name: '' });
	//netproxy("./api/ComplexString", { Name: 'abc def' });
	//netproxy("./api/ComplexStringInt", { Name: 'abc def', A : 123 });
	//netproxy("./api/ComplexList", { list: [ "a", "b" , "", null, "d"] });
	//netproxy("./api/ComplexList", { list: ["a", "", "b", null, "c"] }); // "a" null "b" "b" "c" WTF?
	//netproxy("./api/ComplexListNullableDouble", { list: [1.2, 3.4, 5.6, null, 7.8] });
	//netproxy("./api/ComplexListObjecs", { list: [1.2, null, 3 ] }); // numbers to string, using CultureInfo!
	//netproxy("./api/ComplexStringList", { Name: 'abc def', list: [ 'a', 'b' , 'c'] });
	//netproxy("./api/ComplexObjectArray", { objB: { Name: 'Alphons', List: [{ a: 'a', b: 'b' }, { a: 'c', b: 'd' }] } });
	//netproxy("./api/ComplexArray", { list: [{ a: 'a', b: null }, { a: 'c', b: 'd' }] } );

	//netproxy("./api/ComplexArrayArray",
	//	{
	//		Group : "groupy",
	//		List:
	//		{
	//			Name: "My Name is",
	//			"Users":
	//			[
	//				[ "admins", "1" ],
	//				[ "editors", 2 ], // turns into a string
	//				[ "others", "3" ],
	//				[ "sisters","4" ]
	//			]
	//		}
	//	});

	//netproxy("./api/ComplexArrayArrayClass",
	//	{
	//		Testing: true,
	//		Relaxed: false,
	//		Group: "Nice Group",
	//		GroupInfo:
	//		{
	//			Name: "My Name is",
	//			"Users":
	//				[
	//					[{ Name: "User00", Alias: ['aliasa', 'aliasb', 'aliasc'] }, { Name: "User01" }],
	//					[{ Name: "User10" }, { Name: "User11" }],
	//					[{ Name: "User20" }, { Name: "User21" }]
	//				]
	//		}
	//	});
}
