CREATE TABLE IF NOT EXISTS productfeaturebullets_temp
(
	uniqueid BIGINT, 
	productid INT, 
	localeid SMALLINT, 
	orderid SMALLINT, 
	text TEXT, 
	modifieddate TIMESTAMP,
	PRIMARY KEY(uniqueid),
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);