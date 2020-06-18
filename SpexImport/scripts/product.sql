CREATE TABLE IF NOT EXISTS product_temp
(
	productid INT NOT NULL, 
	manufacturerid INT,
	mfgpn VARCHAR(128) NOT NULL, 
	categoryid INT, 
	isaccessory TINYINT,
	equivalency TEXT, 
	create_date TIMESTAMP, 
	modify_date TIMESTAMP, 
	last_update TIMESTAMP, 
	PRIMARY KEY(productid),
	FOREIGN KEY(manufacturerid) REFERENCES manufacturer(manufacturerid),
	FOREIGN KEY(categoryid) REFERENCES category(categoryid)
);
CREATE INDEX mfg_pn ON product_temp(mfgpn);