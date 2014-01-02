using System;
using System.Linq;
using Ejdb.BSON;
using Ejdb.DB;
using NUnit.Framework;
using System.Collections;

namespace Ejdb.Tests
{
	[TestFixture]
	public class TestCollectionJoins
	{
		private const string ORDERS = "orders";
		private const string CARS = "cars";
		private const string TOPCARS = "topcars";

		private EJDB _db;
		private BsonDocument _car1;
		private BsonDocument _car2;
		private BsonDocument _car3;

		[TestFixtureSetUp]
		public void Setup()
		{
            _db = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);

			_car1 = BsonDocument.ValueOf(new { model = "Honda Accord", year = 2005 });
			_car2 = BsonDocument.ValueOf(new { model = "Toyota Corolla", year = 2011 });
			_car3 = BsonDocument.ValueOf(new { model = "Toyota Camry", year = 2008 });
			_db.Save(CARS, _car1, _car2, _car3);

			var order1 = BsonDocument.ValueOf(new { car = _car1["_id"], pickUpDate = new DateTime(2013, 05, 20), customer = "andy" });
			var order2 = BsonDocument.ValueOf(new { car = _car2["_id"], pickUpDate = new DateTime(2013, 05, 23), customer = "john" });
			var order3 = BsonDocument.ValueOf(new { car = _car2["_id"], pickUpDate = new DateTime(2013, 05, 25), customer = "antony" });
			_db.Save(ORDERS, order1, order2, order3);

			var topCarsJune = BsonDocument.ValueOf(new { month = "June", cars = new[] { _car1["_id"], _car2["_id"] } });
			_db.Save(TOPCARS, topCarsJune);
		}

		[Test]
		public void FindOrdersWithCars()
		{
			var ordersWithCars = _db.Find(ORDERS, Query.Join("car", CARS)).Select(x => x.ToBsonDocument()).ToArray();
			Assert.That(ordersWithCars.Length, Is.EqualTo(3));
			_CheckCar(ordersWithCars[0], _car1);
			_CheckCar(ordersWithCars[1], _car2);
			_CheckCar(ordersWithCars[2], _car2);
		}

		[Test]
		public void FindTopCars()
		{
			var query = Query
				.Join("cars", CARS)
				.EQ("month", "June");
				
			var juneTopCars = _db.Find(TOPCARS, query).Single().ToBsonDocument();

			var topCars = ((object[]) juneTopCars["cars"]).Cast<BsonDocument>().ToArray();
			Assert.That(topCars[0].Equals(_car1));
			Assert.That(topCars[1].Equals(_car2)); 
		}

		private void _CheckCar(BsonDocument orderWithCar, BsonDocument expectedCar)
		{
			var car = (BsonDocument)orderWithCar["car"];
			Assert.That(car.Equals(expectedCar));
		}
	}
}