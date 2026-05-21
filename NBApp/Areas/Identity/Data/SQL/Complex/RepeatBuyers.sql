/*Repeat buyers
Products frequently bought together (co-purchase pairs)
Self-joins OrderItems on the same order to surface product pairs that appear together — the foundation of "customers also bought".*/
SELECT
    p1.Name                 AS ProductA,
    p2.Name                 AS ProductB,
    COUNT(*)               AS TimesBoughtTogether,
    ROUND(
        100.0 * COUNT(*)
              / (SELECT COUNT(DISTINCT OrderId) FROM Orders
                 WHERE Status NOT IN (4))
    , 2)                   AS PctOfAllOrders
FROM  OrderItem oi1
JOIN  OrderItem oi2 ON  oi2.OrderId   = oi1.OrderId
                    AND oi2.ProductId > oi1.ProductId -- avoid dupes & self-pairs
JOIN  Products p1   ON  p1.ProductId  = oi1.ProductId
JOIN  Products p2   ON  p2.ProductId  = oi2.ProductId
JOIN  Orders   o    ON  o.OrderId    = oi1.OrderId
                    AND o.Status NOT IN (4)
GROUP BY p1.Name, p2.Name
HAVING  COUNT(*) >= 2
ORDER BY TimesBoughtTogether DESC;