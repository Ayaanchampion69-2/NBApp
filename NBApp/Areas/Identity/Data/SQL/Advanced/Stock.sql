/*Stock
Products with a sale price that are still in stock
Filters active products where SalePrice is set and stock is available — useful for generating a "deals" page or promotion banner.*/
SELECT
    p.Name,
    p.Price                             AS OriginalPrice,
    p.SalePrice,
    ROUND(
        100.0 * (p.Price - p.SalePrice)
              / p.Price
    , 1)                                AS DiscountPct,
    p.StockQuantity
FROM Products p
WHERE p.IsActive      = 1
  AND p.SalePrice    IS NOT NULL
  AND p.SalePrice    > 0
  AND p.StockQuantity > 0
ORDER BY DiscountPct DESC;