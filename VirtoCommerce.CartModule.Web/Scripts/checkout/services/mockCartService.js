//angular.module('storefrontApp').service('cartService', ['$http', '$q', function ($http, $q) {
//	return {
//		getCart: function () {
//			return $q.resolve({
//				"data": {
//					"storeId": "Clothing",
//					"isAnonymous": false,
//					"customerId": "ab5e60fa-38e8-a219-cca1-06b13861eda1",
//					"currency": "USD",
//					"total": {
//						"currency": {
//							"code": "USD"
//						},
//						"formatedAmount": "$3,280.60"
//					},
//					"subTotal": 10.0000,
//					"subTotalWithTax": {
//						"formatedAmount": "$3,210.60"
//					},
//					"shippingPriceWithTax": {
//						"formatedAmount": "$50"
//					},
//					"discountTotalWithTax": {
//						"formatedAmount": "$15"
//					},
//					"taxTotal": {
//						"formatedAmount": "$15"
//					},
//					"shippingTotal": 0.0000,
//					"handlingTotal": 0.0000,
//					"discountTotal": 0.0000,
//					"email": "",
//					"hasPhysicalProducts": true,
//					"addresses": [],
//					"items": [
//						{
//							"productId": "itemId",
//							"catalogId": "catalogId",
//							"sku": "Sku",
//							"name": "Handcrafted lamp",
//							"quantity": 1,
//							"requiredShipping": false,
//							"imageUrl": "https://virtocommercedemo2.blob.core.windows.net/catalog/1432753636000_1148740.jpg",
//							"isGift": false,
//							"currency": "USD",
//							"isReccuring": false,
//							"taxIncluded": false,
//							"price": {
//								"sale": 250.0000,
//								"currency": "USD"
//							},
//							"placedPriceWithTax": {
//								"formatedAmount": "$1,510.80"
//							},
//							"listPrice": 250.0000,
//							"salePrice": 250.0000,
//							"placedPrice": 250.0000,
//							"extendedPrice": 250.0000,
//							"discountTotal": 0.0000,
//							"taxTotal": 0.0000,
//							"discounts": [],
//							"taxDetails": [],
//							"objectType": "VirtoCommerce.Domain.Cart.Model.LineItem",
//							"dynamicProperties": [],
//							"createdDate": "2016-07-07T12:37:55.18Z",
//							"modifiedDate": "2016-07-07T12:37:55.18Z",
//							"createdBy": "JavaScriptShoppingCartUser@gmail.com",
//							"modifiedBy": "JavaScriptShoppingCartUser@gmail.com",
//							"id": "07ff976bd2434679ba1a607769faaf81"
//						},
//						{
//							"productId": "itemId",
//							"catalogId": "catalogId",
//							"sku": "Sku",
//							"name": "Handcrafted lamp",
//							"quantity": 1,
//							"requiredShipping": false,
//							"imageUrl": "https://virtocommercedemo2.blob.core.windows.net/catalog/1432753636000_1148740.jpg",
//							"isGift": false,
//							"currency": "USD",
//							"isReccuring": false,
//							"taxIncluded": false,
//							"price": {
//								"sale": 250.0000,
//								"currency": "USD"
//							},
//							"placedPriceWithTax": {
//								"formatedAmount": "$1,510.80"
//							},
//							"listPrice": 250.0000,
//							"salePrice": 250.0000,
//							"placedPrice": 250.0000,
//							"extendedPrice": 250.0000,
//							"discountTotal": 250.0000,
//							"taxTotal": 0.0000,
//							"discounts": [],
//							"taxDetails": [],
//							"objectType": "VirtoCommerce.Domain.Cart.Model.LineItem",
//							"dynamicProperties": [],
//							"createdDate": "2016-07-07T12:39:57.533Z",
//							"modifiedDate": "2016-07-07T12:39:57.533Z",
//							"createdBy": "JavaScriptShoppingCartUser@gmail.com",
//							"modifiedBy": "JavaScriptShoppingCartUser@gmail.com",
//							"id": "106e4755a6f54a249c790cfce4ddbf54"
//						}
//					],
//					"payments": [],
//					"shipments": [
//						{
//							"shipmentMethodCode": "ShipmentMethodCode",
//							"shippingPrice": 0.0,
//							"total": 0.0,
//							"discountTotal": 0.0,
//							"taxTotal": 0.0,
//							"itemSubtotal": 0.0,
//							"subtotal": 0.0,
//							"deliveryAddress": {
//								"name": "USA, New York, Workshir 54",
//								"addressType": "shipping",
//								"countryCode": "USA",
//								"countryName": "United States",
//								"city": "New York",
//								"line1": "Workshir 54",
//								"regionName": "Alabama",
//								"firstName": "John",
//								"lastName": "Down",
//								"email": "email@email.com",
//								"postalCode": "45645"
//							},
//							"id": "1",
//							"availShippingMethods": [{ "shipmentMethodCode": "FixedRate", "name": "Fixed rate shipping method", "optionName": "Ground", "optionDescription": "Ground shipping", "logoUrl": "http://somelogo.com/logo.png", "price": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "priceWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "total": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "totalWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotal": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotalWithTax": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "taxTotal": { "internalAmount": 10.0, "amount": 10.0, "truncatedAmount": 10.0, "formatedAmount": "$10.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discounts": [], "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 } }, { "shipmentMethodCode": "FixedRate", "name": "Fixed rate shipping method", "optionName": "Air", "optionDescription": "Air shipping", "logoUrl": "http://somelogo.com/logo.png", "price": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "priceWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "total": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "totalWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotal": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotalWithTax": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "taxTotal": { "internalAmount": 10.0, "amount": 10.0, "truncatedAmount": 10.0, "formatedAmount": "$10.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discounts": [], "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 } }]
//						}
//					],
//					"discounts": [],
//					"taxDetails": [],
//					"objectType": "VirtoCommerce.Domain.Cart.Model.ShoppingCart",
//					"dynamicProperties": [],
//					"createdDate": "2016-07-07T12:22:02.687Z",
//					"modifiedDate": "2016-07-07T12:22:02.687Z",
//					"createdBy": "JavaScriptShoppingCartUser@gmail.com",
//					"modifiedBy": "JavaScriptShoppingCartUser@gmail.com",
//					"id": "af683f2217054a629e11dbb2da6a618e"
//				}
//			});
//		},
//		getCartItemsCount: function () {
//			return $q.resolve(1);
//		},
//		addLineItem: function (item, baseUrl, apiKey) {
//			return $q.resolve({
//				"data": {
//					"cart": {
//						"storeId": "Clothing",
//						"isAnonymous": false,
//						"customerId": "ab5e60fa-38e8-a219-cca1-06b13861eda1",
//						"currency": "USD",
//						"total": {
//							"currency": {
//								"code": "USD"
//							},
//							"formatedAmount": "$10"
//						},
//						"subTotal": 10.0000,
//						"subTotalWithTax": {
//							"formatedAmount": "$3,210.60"
//						},
//						"shippingPriceWithTax": {
//							"formatedAmount": "$1,510.80"
//						},
//						"discountTotalWithTax": {
//							"formatedAmount": "$15"
//						},
//						"taxTotal": {
//							"formatedAmount": "$15"
//						},
//						"shippingTotal": 0.0000,
//						"handlingTotal": 0.0000,
//						"discountTotal": 0.0000,
//						"addresses": [],
//						"hasPhysicalProducts": true,
//						"items": [
//							{
//								"productId": "itemId",
//								"catalogId": "catalogId",
//								"sku": "Sku",
//								"name": "Handcrafted lamp",
//								"quantity": 1,
//								"requiredShipping": false,
//								"imageUrl": "https://virtocommercedemo2.blob.core.windows.net/catalog/1432753636000_1148740.jpg",
//								"isGift": false,
//								"currency": "USD",
//								"isReccuring": false,
//								"taxIncluded": false,
//								"listPrice": 250.0000,
//								"salePrice": 250.0000,
//								"placedPrice": 250.0000,
//								"extendedPrice": 250.0000,
//								"discountTotal": 0.0000,
//								"placedPriceWithTax": {
//									"formatedAmount": "$1,510.80"
//								},
//								"taxTotal": 0.0000,
//								"discounts": [],
//								"taxDetails": [],
//								"objectType": "VirtoCommerce.Domain.Cart.Model.LineItem",
//								"dynamicProperties": [],
//								"createdDate": "2016-07-07T12:37:55.18Z",
//								"modifiedDate": "2016-07-07T12:37:55.18Z",
//								"createdBy": "JavaScriptShoppingCartUser@gmail.com",
//								"modifiedBy": "JavaScriptShoppingCartUser@gmail.com",
//								"id": "07ff976bd2434679ba1a607769faaf81"
//							},
//							{
//								"productId": "itemId",
//								"catalogId": "catalogId",
//								"sku": "Sku",
//								"name": "Handcrafted lamp",
//								"quantity": 1,
//								"requiredShipping": false,
//								"imageUrl": "https://virtocommercedemo2.blob.core.windows.net/catalog/1432753636000_1148740.jpg",
//								"isGift": false,
//								"currency": "USD",
//								"isReccuring": false,
//								"taxIncluded": false,
//								"listPrice": 250.0000,
//								"salePrice": 250.0000,
//								"placedPrice": 250.0000,
//								"extendedPrice": 250.0000,
//								"discountTotal": 250.0000,
//								"placedPriceWithTax": {
//									"formatedAmount": "$1,510.80"
//								},
//								"taxTotal": 0.0000,
//								"discounts": [],
//								"taxDetails": [],
//								"objectType": "VirtoCommerce.Domain.Cart.Model.LineItem",
//								"dynamicProperties": [],
//								"createdDate": "2016-07-07T12:39:57.533Z",
//								"modifiedDate": "2016-07-07T12:39:57.533Z",
//								"createdBy": "JavaScriptShoppingCartUser@gmail.com",
//								"modifiedBy": "JavaScriptShoppingCartUser@gmail.com",
//								"id": "106e4755a6f54a249c790cfce4ddbf54"
//							}
//						],
//						"payments": [],
//						"shipments": [
//							{
//								"shipmentMethodCode": "ShipmentMethodCode",
//								"shippingPrice": 0.0,
//								"total": 0.0,
//								"discountTotal": 0.0,
//								"taxTotal": 0.0,
//								"itemSubtotal": 0.0,
//								"subtotal": 0.0,
//								"deliveryAddress": {
//									"name": "USA, New York, Workshir 54",
//									"addressType": "shipping",
//									"countryCode": "USA",
//									"countryName": "United States",
//									"city": "New York",
//									"line1": "Workshir 54",
//									"regionName": "Alabama",
//									"firstName": "John",
//									"lastName": "Down",
//									"email": "email@email.com",
//									"postalCode": "45645"
//								},
//								"id": "1"
//							}
//						],
//						"discounts": [],
//						"taxDetails": [],
//						"objectType": "VirtoCommerce.Domain.Cart.Model.ShoppingCart",
//						"dynamicProperties": [],
//						"createdDate": "2016-07-07T12:22:02.687Z",
//						"modifiedDate": "2016-07-07T12:22:02.687Z",
//						"createdBy": "JavaScriptShoppingCartUser@gmail.com",
//						"modifiedBy": "JavaScriptShoppingCartUser@gmail.com",
//						"id": "af683f2217054a629e11dbb2da6a618e"
//					},
//					"cartTemplate": "<style>\r\n\t.cart-row img {\r\n\t\tmax-height: 100px;\r\n\t}\r\n\r\n\t.cart-rows {\r\n\t\theight: 300px;\r\n\t\toverflow: auto;\r\n\t}\r\n\r\n\t.modal-header .close-popup {\r\n\t\tmargin: 10px;\r\n\t}\r\n\r\n\t.modal-body {\r\n\t\tmin-height: 300px;\r\n\t}\r\n</style>\r\n\r\n<div>\r\n\t<div class=\"modal-header\">\r\n\t\t<h3 class=\"modal-title pull-left\">SHOPPING CART</h3>\r\n\t\t<span class=\"close-popup glyphicon glyphicon-remove pull-right\" ng-click=\"cancel()\"></span>\r\n\t</div>\r\n\t<div class=\"modal-body\">\r\n\t\t<uib-tabset active=\"active\">\r\n\t\t\t<uib-tab index=\"0\" heading=\"Login\">\r\n\t\t\t\t<div class=\"row\">\r\n\t\t\t\t\t<div class=\"col-md-4\">\r\n\t\t\t\t\t\t<h3>Sign in</h3>\r\n\t\t\t\t\t\t<form>\r\n\t\t\t\t\t\t\t<div class=\"form-group\">\r\n\t\t\t\t\t\t\t\t<label for=\"exampleInputEmail1\">Email</label>\r\n\t\t\t\t\t\t\t\t<input type=\"email\" class=\"form-control\" id=\"exampleInputEmail1\" placeholder=\"Email\">\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t<div class=\"form-group\">\r\n\t\t\t\t\t\t\t\t<label for=\"exampleInputPassword1\">Password</label>\r\n\t\t\t\t\t\t\t\t<input type=\"password\" class=\"form-control\" id=\"exampleInputPassword1\" placeholder=\"Password\">\r\n\t\t\t\t\t\t\t\t<a href=\"#\">I forgot my password</a>\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t<button type=\"submit\" class=\"btn btn-success\">Log in</button>\r\n\t\t\t\t\t\t</form>\r\n\t\t\t\t\t</div>\r\n\t\t\t\t\t<div class=\"col-md-4\">\r\n\t\t\t\t\t\t<h3>Create a login</h3>\r\n\t\t\t\t\t\t<form>\r\n\t\t\t\t\t\t\t<div class=\"form-group\">\r\n\t\t\t\t\t\t\t\t<label for=\"exampleInputEmail1\">Email</label>\r\n\t\t\t\t\t\t\t\t<input type=\"email\" class=\"form-control\" id=\"exampleInputEmail1\" placeholder=\"Email\">\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t<div class=\"form-group\">\r\n\t\t\t\t\t\t\t\t<label for=\"exampleInputPassword1\">Password</label>\r\n\t\t\t\t\t\t\t\t<input type=\"password\" class=\"form-control\" id=\"exampleInputPassword1\" placeholder=\"Password\">\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t<div class=\"form-group\">\r\n\t\t\t\t\t\t\t\t<label for=\"exampleInputPassword1\">Confirm password</label>\r\n\t\t\t\t\t\t\t\t<input type=\"password\" class=\"form-control\" id=\"exampleInputPassword2\" placeholder=\"Confirm password\">\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t<button type=\"submit\" class=\"btn btn-success\">Create a login</button>\r\n\t\t\t\t\t\t</form>\r\n\t\t\t\t\t</div>\r\n\t\t\t\t\t<div class=\"col-md-4\">\r\n\t\t\t\t\t\t<h3>Checkout as a guest</h3>\r\n\t\t\t\t\t\t<form>\r\n\t\t\t\t\t\t\t<div class=\"form-group\">\r\n\t\t\t\t\t\t\t\t<label for=\"exampleInputEmail1\">Email</label>\r\n\t\t\t\t\t\t\t\t<input type=\"email\" class=\"form-control\" id=\"exampleInputEmail1\" placeholder=\"Email\">\r\n\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t<button type=\"submit\" class=\"btn btn-success\">Checkout</button>\r\n\t\t\t\t\t\t</form>\r\n\t\t\t\t\t</div>\r\n\t\t\t\t</div>\r\n\t\t\t</uib-tab>\r\n\t\t\t<uib-tab index=\"1\" heading=\"Items\">\r\n\t\t\t\t<div class=\"cart-rows\">\r\n\t\t\t\t\t<table class=\"table table-hover\">\r\n\t\t\t\t\t\t<thead>\r\n\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t<th>Item</th>\r\n\t\t\t\t\t\t\t\t<th>Quantity</th>\r\n\t\t\t\t\t\t\t\t<th>Unit price</th>\r\n\t\t\t\t\t\t\t\t<th>Total price</th>\r\n\t\t\t\t\t\t\t\t<th></th>\r\n\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t</thead>\r\n\t\t\t\t\t\t<tbody>\r\n\t\t\t\t\t\t\t<tr ng-repeat=\"lineItem in cart.items track by $index\" class=\"cart-row\">\r\n\t\t\t\t\t\t\t\t<td>\r\n\t\t\t\t\t\t\t\t\t<div class=\"row\">\r\n\t\t\t\t\t\t\t\t\t\t<div class=\"col-md-4\">\r\n\t\t\t\t\t\t\t\t\t\t\t<img ng-src=\"{{lineItem.imageUrl}}\" />\r\n\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t\t<div class=\"col-md-8\">\r\n\t\t\t\t\t\t\t\t\t\t\t{{lineItem.name}}\r\n\t\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t\t</div>\r\n\t\t\t\t\t\t\t\t</td>\r\n\t\t\t\t\t\t\t\t<td>{{lineItem.quantity}}</td>\r\n\t\t\t\t\t\t\t\t<td>{{lineItem.price.sale}} {{lineItem.price.currency}}</td>\r\n\t\t\t\t\t\t\t\t<td>{{lineItem.price.sale}} {{lineItem.price.currency}}</td>\r\n\t\t\t\t\t\t\t\t<td><span class=\"glyphicon glyphicon-remove\"></span></td>\r\n\t\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t\t</tbody>\r\n\t\t\t\t\t</table>\r\n\t\t\t\t</div>\r\n\t\t\t</uib-tab>\r\n\t\t\t<uib-tab index=\"2\" heading=\"Shippment method\"></uib-tab>\r\n\t\t\t<uib-tab index=\"3\" heading=\"Billing address\"></uib-tab>\r\n\t\t\t<uib-tab index=\"4\" heading=\"Payment method\"></uib-tab>\r\n\t\t\t<uib-tab index=\"5\" heading=\"Confirm order\">\r\n\t\t\t\t<button type=\"button\" class=\"btn btn-default\" ng-click=\"createOrder()\">Place order</button>\r\n\t\t\t\t<h1>Order number: {{settings.orderNumber}}</h1>\r\n\t\t\t</uib-tab>\r\n\t\t</uib-tabset>\r\n\t</div>\r\n\t<div class=\"modal-footer\">\r\n\t\t<!--<button class=\"btn btn-warning pull-left\" type=\"button\" ng-click=\"cancel()\">Continue shopping</button>-->\r\n\t\t<button class=\"btn btn-warning pull-left disabled\" type=\"button\" ng-click=\"previousStep()\">Previous step</button>\r\n\t\t<button class=\"btn btn-warning pull-right\" type=\"button\" ng-click=\"nextStep()\">Next step</button>\r\n\t</div>\r\n</div>\r\n"
//				}
//			});
//		},
//		getCountries: function () {
//			return $q.resolve({
//				"data": [{ "name": "Canada", "code2": "CA", "code3": "CAN", "regionType": "Province" }, { "name": "United States", "code2": "US", "code3": "USA", "regionType": "State" }, { "name": "United Kingdom", "code2": "GB", "code3": "GBR", "regionType": "Region" }, { "name": "Albania", "code2": "AL", "code3": "ALB", "regionType": "Region" }, { "name": "Algeria", "code2": "DZ", "code3": "DZA", "regionType": "Province" }, { "name": "Andorra", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Angola", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Argentina", "code2": "AR", "code3": "ARG", "regionType": "Province" }, { "name": "Armenia", "code2": "AM", "code3": "ARM", "regionType": "Region" }, { "name": "Aruba", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Antigua And Barbuda", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Australia", "code2": "AU", "code3": "AUS", "regionType": "State/Territory" }, { "name": "Austria", "code2": "AT", "code3": "AUT", "regionType": "Region" }, { "name": "Azerbaijan", "code2": "AZ", "code3": "AZE", "regionType": "Region" }, { "name": "Bangladesh", "code2": "BD", "code3": "BGD", "regionType": "Region" }, { "name": "Bahamas", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Bahrain", "code2": "BH", "code3": "BHR", "regionType": "Region" }, { "name": "Barbados", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Belarus", "code2": "BY", "code3": "BLR", "regionType": "Province" }, { "name": "Belgium", "code2": "BE", "code3": "BEL", "regionType": "Region" }, { "name": "Belize", "code2": "BZ", "code3": "BLZ", "regionType": "Region" }, { "name": "Bermuda", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Bhutan", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Bolivia", "code2": "BO", "code3": "BOL", "regionType": "Region" }, { "name": "Bosnia And Herzegovina", "code2": "BA", "code3": "BIH", "regionType": "Region" }, { "name": "Botswana", "code2": "BW", "code3": "BWA", "regionType": "Region" }, { "name": "Brazil", "code2": "BR", "code3": "BRA", "regionType": "State" }, { "name": "Brunei", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Bulgaria", "code2": "BG", "code3": "BGR", "regionType": "Region" }, { "name": "Cambodia", "code2": "KH", "code3": "KHM", "regionType": "Region" }, { "name": "Republic of Cameroon", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Cayman Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Chile", "code2": "CL", "code3": "CHL", "regionType": "State" }, { "name": "China", "code2": "CN", "code3": "CHN", "regionType": "Region" }, { "name": "Colombia", "code2": "CO", "code3": "COL", "regionType": "Region" }, { "name": "Comoros", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Congo", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Costa Rica", "code2": "CR", "code3": "CRI", "regionType": "Region" }, { "name": "Côte d'Ivoire", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Croatia", "code2": "HR", "code3": "HRV", "regionType": "Region" }, { "name": "Curaçao", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Cyprus", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Czech Republic", "code2": "CZ", "code3": "CZE", "regionType": "Region" }, { "name": "Denmark", "code2": "DK", "code3": "DNK", "regionType": "Region" }, { "name": "Dominica", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Dominican Republic", "code2": "DO", "code3": "DOM", "regionType": "Region" }, { "name": "Ecuador", "code2": "EC", "code3": "ECU", "regionType": "Region" }, { "name": "Egypt", "code2": "EG", "code3": "EGY", "regionType": "Governorate" }, { "name": "El Salvador", "code2": "SV", "code3": "SLV", "regionType": "Region" }, { "name": "Estonia", "code2": "EE", "code3": "EST", "regionType": "Region" }, { "name": "Ethiopia", "code2": "ET", "code3": "ETH", "regionType": "Region" }, { "name": "Faroe Islands", "code2": "FO", "code3": "FRO", "regionType": "Region" }, { "name": "Fiji", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Finland", "code2": "FI", "code3": "FIN", "regionType": "Region" }, { "name": "France", "code2": "FR", "code3": "FRA", "regionType": "Region" }, { "name": "French Polynesia", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Gambia", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Georgia", "code2": "GE", "code3": "GEO", "regionType": "Region" }, { "name": "Germany", "code2": "DE", "code3": "DEU", "regionType": "Region" }, { "name": "Ghana", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Gibraltar", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Greece", "code2": "GR", "code3": "GRC", "regionType": "Region" }, { "name": "Greenland", "code2": "GL", "code3": "GRL", "regionType": "Region" }, { "name": "Grenada", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Guadeloupe", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Guatemala", "code2": "GT", "code3": "GTM", "regionType": "Region" }, { "name": "Guernsey", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Guyana", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Haiti", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Honduras", "code2": "HN", "code3": "HND", "regionType": "Region" }, { "name": "Hong Kong", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Hungary", "code2": "HU", "code3": "HUN", "regionType": "Region" }, { "name": "Iceland", "code2": "IS", "code3": "ISL", "regionType": "Region" }, { "name": "India", "code2": "IN", "code3": "IND", "regionType": "State" }, { "name": "Indonesia", "code2": "ID", "code3": "IDN", "regionType": "Province" }, { "name": "Ireland", "code2": "IE", "code3": "IRL", "regionType": "Region" }, { "name": "Isle Of Man", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Israel", "code2": "IL", "code3": "ISR", "regionType": "Region" }, { "name": "Italy", "code2": "IT", "code3": "ITA", "regionType": "Province" }, { "name": "Jamaica", "code2": "JM", "code3": "JAM", "regionType": "Region" }, { "name": "Japan", "code2": "JP", "code3": "JPN", "regionType": "Prefecture" }, { "name": "Jersey", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Jordan", "code2": "JO", "code3": "JOR", "regionType": "Region" }, { "name": "Kazakhstan", "code2": "KZ", "code3": "KAZ", "regionType": "Region" }, { "name": "Kenya", "code2": "KE", "code3": "KEN", "regionType": "Region" }, { "name": "Kosovo", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Kuwait", "code2": "KW", "code3": "KWT", "regionType": "Region" }, { "name": "Kyrgyzstan", "code2": "KG", "code3": "KGZ", "regionType": "Region" }, { "name": "Latvia", "code2": "LV", "code3": "LVA", "regionType": "Region" }, { "name": "Lebanon", "code2": "LB", "code3": "LBN", "regionType": "Region" }, { "name": "Liberia", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Liechtenstein", "code2": "LI", "code3": "LIE", "regionType": "Region" }, { "name": "Lithuania", "code2": "LT", "code3": "LTU", "regionType": "Region" }, { "name": "Luxembourg", "code2": "LU", "code3": "LUX", "regionType": "Region" }, { "name": "Macao", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Macedonia, Republic Of", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Madagascar", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Malaysia", "code2": "MY", "code3": "MYS", "regionType": "State/Territory" }, { "name": "Maldives", "code2": "MV", "code3": "MDV", "regionType": "Region" }, { "name": "Malta", "code2": "MT", "code3": "MLT", "regionType": "Region" }, { "name": "Mauritius", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Mexico", "code2": "MX", "code3": "MEX", "regionType": "State" }, { "name": "Moldova, Republic of", "code2": "", "code3": "", "regionType": "" }, { "name": "Monaco", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Mongolia", "code2": "MN", "code3": "MNG", "regionType": "" }, { "name": "Montenegro", "code2": "ME", "code3": "MNE", "regionType": "Region" }, { "name": "Morocco", "code2": "MA", "code3": "MAR", "regionType": "Region" }, { "name": "Mozambique", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Myanmar", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Namibia", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Nepal", "code2": "NP", "code3": "NPL", "regionType": "Region" }, { "name": "Netherlands Antilles", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Netherlands", "code2": "NL", "code3": "NLD", "regionType": "Region" }, { "name": "New Zealand", "code2": "NZ", "code3": "NZL", "regionType": "Region" }, { "name": "Nicaragua", "code2": "NI", "code3": "NIC", "regionType": "Region" }, { "name": "Niger", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Nigeria", "code2": "NG", "code3": "NGA", "regionType": "State" }, { "name": "Norway", "code2": "NO", "code3": "NOR", "regionType": "Region" }, { "name": "Oman", "code2": "OM", "code3": "OMN", "regionType": "Region" }, { "name": "Pakistan", "code2": "PK", "code3": "PAK", "regionType": "Region" }, { "name": "Palestinian Territory, Occupied", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Panama", "code2": "PA", "code3": "PAN", "regionType": "Region" }, { "name": "Papua New Guinea", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Paraguay", "code2": "PY", "code3": "PRY", "regionType": "" }, { "name": "Peru", "code2": "PE", "code3": "PER", "regionType": "Region" }, { "name": "Philippines", "code2": "PH", "code3": "PHL", "regionType": "Region" }, { "name": "Poland", "code2": "PL", "code3": "POL", "regionType": "Region" }, { "name": "Portugal", "code2": "PT", "code3": "PRT", "regionType": "Region" }, { "name": "Qatar", "code2": "QA", "code3": "QAT", "regionType": "Region" }, { "name": "Reunion", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Romania", "code2": "RO", "code3": "ROU", "regionType": "Region" }, { "name": "Russia", "code2": "RU", "code3": "RUS", "regionType": "Region" }, { "name": "Rwanda", "code2": "RW", "code3": "RWA", "regionType": "Region" }, { "name": "Saint Kitts And Nevis", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Saint Lucia", "code2": "", "code3": "", "regionType": "" }, { "name": "Saint Martin", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Sao Tome And Principe", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Samoa", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Saudi Arabia", "code2": "SA", "code3": "SAU", "regionType": "Region" }, { "name": "Senegal", "code2": "SN", "code3": "SEN", "regionType": "Region" }, { "name": "Serbia", "code2": "RS", "code3": "SRB", "regionType": "Region" }, { "name": "Seychelles", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Singapore", "code2": "SG", "code3": "SGP", "regionType": "Region" }, { "name": "Sint Maarten", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Slovakia", "code2": "SK", "code3": "SVK", "regionType": "Region" }, { "name": "Slovenia", "code2": "SI", "code3": "SVN", "regionType": "Region" }, { "name": "South Africa", "code2": "ZA", "code3": "ZAF", "regionType": "Province" }, { "name": "South Korea", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Spain", "code2": "ES", "code3": "ESP", "regionType": "Province" }, { "name": "Sri Lanka", "code2": "LK", "code3": "LKA", "regionType": "Region" }, { "name": "St. Vincent", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Suriname", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Sweden", "code2": "SE", "code3": "SWE", "regionType": "Region" }, { "name": "Switzerland", "code2": "CH", "code3": "CHE", "regionType": "Region" }, { "name": "Syria", "code2": "SY", "code3": "SYR", "regionType": "Region" }, { "name": "Taiwan", "code2": "TW", "code3": "TWN", "regionType": "Region" }, { "name": "Thailand", "code2": "TH", "code3": "THA", "regionType": "Region" }, { "name": "Tanzania, United Republic Of", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Trinidad and Tobago", "code2": "TT", "code3": "TTO", "regionType": "Region" }, { "name": "Tunisia", "code2": "TN", "code3": "TUN", "regionType": "Region" }, { "name": "Turkey", "code2": "TR", "code3": "TUR", "regionType": "Region" }, { "name": "Turkmenistan", "code2": "TM", "code3": "TKM", "regionType": "Region" }, { "name": "Turks and Caicos Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Uganda", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Ukraine", "code2": "UA", "code3": "UKR", "regionType": "Region" }, { "name": "United Arab Emirates", "code2": "", "code3": "", "regionType": "Emirate" }, { "name": "Uruguay", "code2": "UY", "code3": "URY", "regionType": "Region" }, { "name": "Uzbekistan", "code2": "UZ", "code3": "UZB", "regionType": "Province" }, { "name": "Vanuatu", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Venezuela", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Vietnam", "code2": "VN", "code3": "VNM", "regionType": "Region" }, { "name": "Virgin Islands, British", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Yemen", "code2": "YE", "code3": "YEM", "regionType": "Region" }, { "name": "Zambia", "code2": "", "code3": "", "regionType": "" }, { "name": "Zimbabwe", "code2": "ZW", "code3": "ZWE", "regionType": "Region" }, { "name": "Afghanistan", "code2": "AF", "code3": "AFG", "regionType": "Region" }, { "name": "Aland Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Anguilla", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Benin", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Bouvet Island", "code2": "", "code3": "", "regionType": "Region" }, { "name": "British Indian Ocean Territory", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Burkina Faso", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Burundi", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Cape Verde", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Central African Republic", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Chad", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Christmas Island", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Cocos (Keeling) Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Congo, The Democratic Republic Of The", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Cook Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Cuba", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Djibouti", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Equatorial Guinea", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Eritrea", "code2": "ER", "code3": "ERI", "regionType": "Region" }, { "name": "Falkland Islands (Malvinas)", "code2": "", "code3": "", "regionType": "Region" }, { "name": "French Guiana", "code2": "", "code3": "", "regionType": "Region" }, { "name": "French Southern Territories", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Gabon", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Guinea", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Guinea Bissau", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Heard Island And Mcdonald Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Holy See (Vatican City State)", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Iran, Islamic Republic Of", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Iraq", "code2": "IQ", "code3": "IRQ", "regionType": "Region" }, { "name": "Kiribati", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Korea, Democratic People's Republic Of", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Lao People's Democratic Republic", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Lesotho", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Libyan Arab Jamahiriya", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Malawi", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Mali", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Martinique", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Mauritania", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Mayotte", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Montserrat", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Nauru", "code2": "", "code3": "", "regionType": "Region" }, { "name": "New Caledonia", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Niue", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Norfolk Island", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Pitcairn", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Saint Barthélemy", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Saint Helena", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Saint Pierre And Miquelon", "code2": "", "code3": "", "regionType": "Region" }, { "name": "San Marino", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Sierra Leone", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Solomon Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Somalia", "code2": "", "code3": "", "regionType": "Region" }, { "name": "South Georgia And The South Sandwich Islands", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Sudan", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Svalbard And Jan Mayen", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Swaziland", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Tajikistan", "code2": "TJ", "code3": "TAJ", "regionType": "Region" }, { "name": "Timor Leste", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Togo", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Tokelau", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Tonga", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Tuvalu", "code2": "", "code3": "", "regionType": "Region" }, { "name": "United States Minor Outlying Islands", "code2": "", "code3": "", "regionType": "State" }, { "name": "Wallis And Futuna", "code2": "", "code3": "", "regionType": "Region" }, { "name": "Western Sahara", "code2": "", "code3": "", "regionType": "Region" }]
//			});
//		},
//		getCountryRegions: function (countryCode) {
//			return $q.resolve({
//				"data": [{ "name": "Alabama", "code": "AL" }, { "name": "Alaska", "code": "AK" }, { "name": "American Samoa", "code": "AS" }, { "name": "Arizona", "code": "AZ" }, { "name": "Arkansas", "code": "AR" }]
//			});
//		},
//		getAvailableShippingMethods: function (shipmentId) {
//			return $q.resolve({
//				"data": [{ "shipmentMethodCode": "FixedRate", "name": "Fixed rate shipping method", "optionName": "Ground", "optionDescription": "Ground shipping", "logoUrl": "http://somelogo.com/logo.png", "price": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "priceWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "total": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "totalWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotal": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotalWithTax": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "taxTotal": { "internalAmount": 10.0, "amount": 10.0, "truncatedAmount": 10.0, "formatedAmount": "$10.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discounts": [], "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 } }, { "shipmentMethodCode": "FixedRate", "name": "Fixed rate shipping method", "optionName": "Air", "optionDescription": "Air shipping", "logoUrl": "http://somelogo.com/logo.png", "price": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "priceWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "total": { "internalAmount": 50.0, "amount": 50.0, "truncatedAmount": 50.0, "formatedAmount": "$50.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "totalWithTax": { "internalAmount": 60.0, "amount": 60.0, "truncatedAmount": 60.0, "formatedAmount": "$60.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotal": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discountTotalWithTax": { "internalAmount": 0.0, "amount": 0.0, "truncatedAmount": 0.0, "formatedAmount": "$0.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "taxTotal": { "internalAmount": 10.0, "amount": 10.0, "truncatedAmount": 10.0, "formatedAmount": "$10.00", "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 }, "decimalDigits": 2 }, "discounts": [], "currency": { "code": "USD", "cultureName": "en-US", "symbol": "$", "englishName": "US dollar", "exchangeRate": 1.0 } }]
//			});
//		},
//		getAvailablePaymentMethods: function () {
//			return $q.resolve({
//				"data": [{ "gatewayCode": "DefaultManualPaymentMethod", "name": "Manual test, don't use on production", "iconUrl": "http://virtocommerce.com/Content/images/logo.jpg", "description": "Manual test, don't use on production", "type": "Unknown", "group": "Manual", "priority": 0, "isAvailableForPartial": false }]
//			});
//		},
//		changeLineItemQuantity: function (lineItemId, quantity) {
//			return $q.resolve();
//		},
//		removeLineItem: function (lineItemId) {
//			return $q.resolve();
//		},
//		clearCart: function () {
//			return $q.resolve();
//		},
//		addCoupon: function (couponCode) {
//			return $q.resolve();
//		},
//		removeCoupon: function () {
//			return $q.resolve();
//		},
//		addOrUpdateShipment: function (shipment) {
//			return $q.resolve();
//		},
//		addOrUpdatePayment: function (payment) {
//			return $q.resolve();
//		},
//		createOrder: function (cartId) {
//			return $q.resolve({
//				"data": {
//					order: { number: "#777" }
//				}
//			});
//		}
//	}
//}]);

