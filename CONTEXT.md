# Atelier Store

The storefront for Atelier, a fashion house: browsing the catalog, product pages, and editorial content.

## Language

### Catalog

**Product**:
A single piece sold in the store, identified by its lowercase slug.
_Avoid_: Item, SKU, article

**Category**:
The one group a Product belongs to (for example Outerwear or Bags); drives Related Products.
_Avoid_: Department, type

**Stock State**:
How available a Product is: In Stock, Low Stock (3 or fewer units left), or Sold Out (none left, or no stock recorded).
_Avoid_: Availability, inventory status

**Badge**:
A short merchandising label on a Product, such as "New". Sold Out always wins: the product card shows "Sold out" in the badge's place, and the product page shows no badge at all.
_Avoid_: Tag, label, flag

**Was Price**:
The original price of a Product on sale, always higher than its current price.
_Avoid_: Old price, list price, compare-at price

### Storefront

**New Arrivals**:
The most recently added Products, newest first ("New in" on the site).
_Avoid_: Latest, new products

**Related Products**:
Products suggested alongside another: same Category first, then newest.
_Avoid_: Recommendations, similar items

**Featured Collection**:
An editorial tile linking to a curated area of the store (Women, Men, Shoes, Jewelry). Editorial content, not catalog data.
_Avoid_: Category (a different concept)
