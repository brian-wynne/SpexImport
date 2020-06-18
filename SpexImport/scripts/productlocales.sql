CREATE TABLE IF NOT EXISTS productlocales_temp
(
	productid INT, 
	isactive CHAR(1), 
	published TINYTEXT, 
	FOREIGN KEY(productid) REFERENCES product(productid)
);