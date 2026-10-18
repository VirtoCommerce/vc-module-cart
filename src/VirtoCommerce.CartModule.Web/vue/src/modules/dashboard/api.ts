// The cart search the dashboard widget reads (same-origin, cookie auth): a plain fetch, as the plugin needs one call

/**
 * A date as the platform's search criteria take it: UTC without the "Z". ASP.NET would bind a "Z" date into server-local
 * time, moving it by the server's UTC offset.
 */
const criteriaDate = (date: Date) => date.toISOString().replace("Z", "");

/** Carts with items changed in the last `days` days, of registered customers or of guests (wishlists and saved-for-later lists excluded) */
export async function countCarts(isAnonymous: boolean, days = 30): Promise<number> {
  const response = await fetch("/api/carts/search", {
    method: "POST",
    headers: { Accept: "application/json", "Content-Type": "application/json" },
    body: JSON.stringify({
      hasLineItems: true,
      notTypes: ["Wishlist", "SavedForLater"],
      modifiedStartDate: criteriaDate(new Date(Date.now() - days * 86_400_000)),
      isAnonymous,
      take: 0,
    }),
  });
  if (!response.ok) throw new Error(`${response.status} ${response.statusText}: /api/carts/search`);
  const result = (await response.json()) as { totalCount?: number };
  return result.totalCount ?? 0;
}
