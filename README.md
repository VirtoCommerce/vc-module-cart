# Virto Commerce Cart Module

[![CI status](https://github.com/VirtoCommerce/vc-module-cart/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-cart/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-cart&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-cart) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-cart&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-cart) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-cart&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-cart) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-cart&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-cart)

The Virto Commerce Cart module represents shopping cart management system. This module doesn't have any UI in VC Manager.

The Virto Commerce Cart module manages customers accumulated list of items, calculates a total for the order, including shipping and handling (i.e., postage and packing) charges and the associated taxes.

![Cart Module](docs/media/screen-cart-module.png)

![Cart Module Info](docs/media/screen-cart-module-info.png)

## Key features

* **Multiple carts support**: If the user is using more than one cart at the same time, all of the carts will be supported by the Virto Commerce Cart module.
* **Named lists**: The user can add the desired products to the  wishlist , which will be saved in the cart. The user can later use the products added to the wishlist to make an order. The wishlist details will be saved by the VirtoCommerce.Cart module.
* **Grouping multiple carts to one order**: If the user is using more than one cart, the final order will be created by grouping the details from all the carts filled out by user.
* **Anonymous carts**: The user can put products to cart and submit order anonymously, without creating an account.
* **Stock reservation**: The selected products will be reserved in stock after adding them to the cart.
* **Multiple payments methods**: The user can choose the payment methods he wants before checkout.
* **Create new cart from orders history**: The user can add products to a new cart using the previously completed orders (orders history).

## Documentation

* [Cart Module user documentation](https://docs.virtocommerce.org/platform/user-guide/cart/overview/)
* [GraphQL API documentation](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/Cart/overview/)
* [REST API](https://virtostart-demo-admin.govirto.com/docs/index.html?urls.primaryName=VirtoCommerce.Cart)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-cart/)

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-cart/releases/latest)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
