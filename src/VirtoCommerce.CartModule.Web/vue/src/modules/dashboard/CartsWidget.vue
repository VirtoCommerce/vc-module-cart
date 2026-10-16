<!-- Currently in carts (DashboardVisuals.jsx CurrentlyInCarts): carts with items changed in the last 30 days,
     split into registered customers and guests (wishlists and saved-for-later lists excluded) -->
<template>
  <DashboardWidgetCard
    :header="$t('CART.DASHBOARD.CARTS.TITLE')"
    icon="lucide-shopping-basket"
    :loading="loading"
  >
    <template #content>
      <div class="dash-donut">
        <div class="dash-donut__chart">
          <svg
            width="200"
            height="200"
            viewBox="0 0 200 200"
            role="img"
            :aria-label="`${total} ${$t('CART.DASHBOARD.CARTS.CARTS')}`"
          >
            <circle
              v-if="!total"
              cx="100"
              cy="100"
              :r="R"
              fill="none"
              style="stroke: var(--border-row)"
              stroke-width="18"
            />
            <g
              v-else
              transform="rotate(-90 100 100)"
            >
              <circle
                v-for="arc in arcs"
                :key="arc.color"
                cx="100"
                cy="100"
                :r="R"
                fill="none"
                :stroke="arc.color"
                stroke-width="18"
                stroke-linecap="round"
                :stroke-dasharray="`${arc.length} ${CIRCUMFERENCE - arc.length}`"
                :stroke-dashoffset="-arc.offset"
              />
            </g>
          </svg>
          <div class="dash-donut__center">
            <span class="dash-donut__total">{{ total }}</span>
            <span class="dash-donut__unit">{{ $t("CART.DASHBOARD.CARTS.CARTS") }}</span>
          </div>
        </div>
        <div class="dash-donut__legend">
          <div
            v-for="segment in segments"
            :key="segment.key"
            class="dash-donut__legend-row"
          >
            <span
              class="dash-donut__swatch"
              :style="{ background: segment.color }"
            />
            <span>
              <span class="dash-donut__label">{{ $t(`CART.DASHBOARD.CARTS.${segment.key}`) }}</span>
              <span class="dash-donut__count">{{ segment.count }}</span>
            </span>
          </div>
        </div>
      </div>
    </template>
  </DashboardWidgetCard>
</template>

<script lang="ts" setup>
import { DashboardWidgetCard, useAsync } from "@vc-shell/framework";
import { computed, onMounted, ref } from "vue";
import { countCarts } from "./api";

const R = 80;
const CIRCUMFERENCE = 2 * Math.PI * R;
const GAP = 6;

const registered = ref(0);
const guests = ref(0);

// the app's error handler shows a failure
const { loading, action: load } = useAsync(
  async () => {
    [registered.value, guests.value] = await Promise.all([countCarts(false), countCarts(true)]);
  },
  { notify: false },
);
onMounted(load);

const total = computed(() => registered.value + guests.value);
const segments = computed(() => [
  { key: "REGISTERED", count: registered.value, color: "#319ED4" },
  { key: "GUESTS", count: guests.value, color: "#A9D5EC" },
]);

// arcs with rounded caps and a small gap between them, as in the design
const arcs = computed(() => {
  let offset = 0;
  return segments.value
    .filter((s) => s.count)
    .map((s) => {
      const share = (s.count / total.value) * CIRCUMFERENCE;
      const length = Math.max(0, share - (s.count === total.value ? 0 : GAP));
      const arc = { color: s.color, length, offset };
      offset += share;
      return arc;
    });
});
</script>
