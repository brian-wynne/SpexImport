CREATE TABLE IF NOT EXISTS productkeywords_temp
(
	productid INT, 
	text TEXT, 
	localeid SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);