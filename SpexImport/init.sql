CREATE TABLE IF NOT EXISTS product 
(
	productid INT NOT NULL, 
	mfgid VARCHAR(16), 
	mfgpn VARCHAR(128) NOT NULL, 
	categoryid INT, 
	is_active CHAR(1), 
	equivalency TEXT, 
	create_date TIMESTAMP, 
	modify_date TIMESTAMP, 
	last_update TIMESTAMP, 
	PRIMARY KEY(productid, mfgpn)
);
CREATE UNIQUE INDEX product_id ON product(productid);

CREATE TABLE IF NOT EXISTS product_attributes 
(
	productid INT, 
	attributeid BIGINT, 
	setnumber SMALLINT, 
	text TEXT, 
	absolutevalue DOUBLE, 
	unitid INT, 
	isabsolute SMALLINT, 
	isactive SMALLINT, 
	localeid INT, 
	type INT
);
CREATE INDEX product_id ON product_attributes(productid);

CREATE TABLE IF NOT EXISTS product_descriptions 
(
	productid INT, 
	description TEXT, 
	isdefault CHAR(1), 
	type CHAR(1), 
	localeid CHAR(1)
);
CREATE INDEX product_id ON product_descriptions(productid);

CREATE TABLE IF NOT EXISTS product_featurebullets 
(
	uniqueid BIGINT, 
	productid INT, 
	localeid SMALLINT, 
	orderid SMALLINT, 
	text TEXT, 
	modifieddate TIMESTAMP
);
CREATE INDEX product_id ON product_featurebullets(productid);

CREATE TABLE IF NOT EXISTS product_locales 
(
	productid INT, 
	isactive CHAR(1), 
	published TINYTEXT, 
	PRIMARY KEY(productid)
);
CREATE INDEX product_id ON product_locales(productid);

CREATE TABLE IF NOT EXISTS product_accessories 
(
	productid INT, 
	accessoryid INT, 
	localeid SMALLINT
);
CREATE INDEX product_id ON product_accessories(productid);

CREATE TABLE IF NOT EXISTS search_attributes 
(
	productid INT, 
	categoryid INT, 
	unknownid INT, 
	isactive SMALLINT, 
	localeid SMALLINT
);
CREATE INDEX product_id ON search_attributes(productid);

CREATE TABLE IF NOT EXISTS product_keywords 
(
	productid INT, 
	text TEXT, 
	localeid SMALLINT
);
CREATE INDEX product_id ON product_keywords(productid);