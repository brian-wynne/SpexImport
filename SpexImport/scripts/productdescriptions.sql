CREATE TABLE IF NOT EXISTS productdescriptions_temp
(
	productid INT, 
	description TEXT, 
	isdefault CHAR(1), 
	type CHAR(1), 
	localeid SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);