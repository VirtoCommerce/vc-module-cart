// Cart widgets on the Admin Shell dashboard. Every widget declares a position (the grid only honours declared
// positions when all widgets have one); the rows follow the design's Dashboard.jsx: carts sit at y10, right of
// Orders' "Recent orders".
import { registerDashboardWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import CartsWidget from "./CartsWidget.vue";

registerDashboardWidget({
  id: "cart-current-carts",
  name: "Currently in carts",
  component: markRaw(CartsWidget),
  size: { width: 4, height: 6 },
  position: { x: 8, y: 10 },
  permissions: ["cart:read"],
});
