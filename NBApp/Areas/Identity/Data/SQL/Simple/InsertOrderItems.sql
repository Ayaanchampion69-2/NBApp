INSERT INTO OrderItem (Quantity, UnitPrice, OrderId, ProductId) VALUES
-- Order 3: B&J + Gelato
(2, 4.50,  3, 16),
(1, 9.50,  3, 21),
-- Order 4: MagnumRipOff + Popsicle + Matcha
(2, 9.50,  4, 24),
(2, 3.75,  4, 26),
(1, 12.00, 4, 25),
-- Order 19: B&J + Gelato (same pair — feeds co-purchase query)
(3, 4.50,  19, 16),
(1, 9.50,  19, 21),
-- Order 20: BlueBell + KitKatIce + Matcha
(2, 4.50,  20, 17),
(1, 6.50,  20, 23),
(1, 12.00, 20, 25),
-- Order 21: SnowCone
(3, 4.50,  21, 27),
-- Order 22: FrozenCoffeeIce + Popsicle
(2, 6.50,  22, 20),
(1, 3.75,  22, 26),
-- Order 23: Chococream + Novelty
(2, 4.50,  23, 28),
(1, 5.00,  23, 29);