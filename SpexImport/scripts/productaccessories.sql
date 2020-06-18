CREATE TABLE IF NOT EXISTS productaccessories_temp
(
	productid INT, 
	accessoryid INT, 
	localeid SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);