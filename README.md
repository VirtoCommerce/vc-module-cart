# VirtoCommerce.Cart

## Overview

VirtoCommerce.Cart module represents shopping cart management system. This module doesn't have any UI in VC Manager.

Shopping cart module manages customers accumulated list of items, calculates a total for the order, including shipping and handling (i.e., postage and packing) charges and the associated taxes.

![Cart Module](docs/media/screen-cart-module.png)

![Cart Module Info](docs/media/screen-cart-module-info.png)

## Features

1. Supports multiple carts
1. Wishlist
1. Grouping multiple carts to one order
1. Anonymous carts
1. Stock reservation
1. Multiple payments methods
1. Create new cart from orders history

The main purpose of the cart module is to save and store the shopping cart data and changes on VC Manager side.

The cart module is connected with the shopping cart via two types of API requests:

1. General API request that allows saving the entire shopping cart information on the database;
1. Specific API requests that allow call specific operations, for example delete, edit, choose payment method, choose delivery type, etc.

For more details about the available cart-module API, please follow the link bellow:

https://admin-demo.virtocommerce.com/docs/ui/index#/Shopping%2520cart%2520module

The cart-module functionality can be extended and customized based on the specific business needs. In order to extend the cart module functionality, use the carte module template that can be accessed by following the link below:

https://marketplace.visualstudio.com/items?itemName=VirtoCommerce2xModuleProjectTemplate.VirtoCommerce2xModule


## Installation
Installing the module:
* Automatically: in VC Manager go to Configuration -> Modules -> Shopping cart module -> Install
* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-cart/releases. In VC Manager go to Configuration -> Modules -> Advanced -> upload module package -> Install.

## Available resources
* Module related service implementations as a <a href="https://www.nuget.org/packages/VirtoCommerce.CartModule.Data" target="_blank">NuGet package</a>
* API client as a <a href="https://www.nuget.org/packages/VirtoCommerce.CartModule.Client" target="_blank">NuGet package</a>
* API client documentation http://admin-demo.virtocommerce.com/docs/ui/index#!/Shopping_cart_module

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
