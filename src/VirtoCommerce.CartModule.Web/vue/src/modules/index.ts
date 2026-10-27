import { defineAppModule } from "@vc-shell/framework";
import * as locales from "./locales";
// registers the dashboard widgets (must run before the host builds the dashboard)
import "./dashboard";

// The Cart module has no admin screens; its plugin brings its Admin Shell dashboard widget and texts
export default defineAppModule({
  locales,
});
