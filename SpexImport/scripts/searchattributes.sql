CREATE TABLE IF NOT EXISTS searchattributes_temp
(
	productid INT, 
	attributeid BIGINT, 
	valueid INT, 
	localeid SMALLINT, 
	setnumber SMALLINT,
	isactive SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid),
	FOREIGN KEY(valueid) REFERENCES searchattributevalues(valueid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);