DROP DATABASE IF EXISTS spex;
CREATE DATABASE spex;
USE spex;

-- Index tables
CREATE TABLE IF NOT EXISTS manufacturer
(
	manufacturerid INT,
	name TEXT,
	url TEXT,
	logowidth INT,
	logoheight INT,
	PRIMARY KEY(manufacturerid)
);

CREATE TABLE IF NOT EXISTS locales 
(
	localeid SMALLINT,
	isactive TINYINT,
	languagecode VARCHAR(5),
	countrycode VARCHAR(5),
	name TEXT,
	PRIMARY KEY(localeid)
);

CREATE TABLE IF NOT EXISTS units
(
	unitid INT,
	baseunitid INT,
	mutliple DOUBLE,
	PRIMARY KEY(unitid)
);

CREATE TABLE IF NOT EXISTS unitnames
(
	unitid INT,
	name TEXT,
	localeid SMALLINT,
	FOREIGN KEY(unitid) REFERENCES units(unitid)
);

CREATE TABLE IF NOT EXISTS category 
(
	categoryid INT,
	parentcategoryid INT,
	isactive SMALLINT,
	ordernumber SMALLINT,
	catlevel SMALLINT,
	PRIMARY KEY(categoryid)
);

ALTER TABLE category ADD FOREIGN KEY(parentcategoryid) REFERENCES category(categoryid);

CREATE TABLE IF NOT EXISTS product 
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
CREATE INDEX mfg_pn ON product(mfgpn);

CREATE TABLE IF NOT EXISTS headernames
(
	headerid INT,
	name TEXT,
	localeid SMALLINT,
	PRIMARY KEY(headerid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

CREATE TABLE IF NOT EXISTS productfeaturebullets 
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

CREATE TABLE IF NOT EXISTS attributenames
(
	attributeid BIGINT,
	name TEXT,
	localeid SMALLINT,
	PRIMARY KEY(attributeid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

-- Non-Index

CREATE TABLE IF NOT EXISTS productattributes 
(
	productid INT, 
	attributeid BIGINT, 
	setnumber SMALLINT, 
	text TEXT, 
	absolutevalue DOUBLE, 
	unitid INT, 
	isabsolute SMALLINT, 
	isactive SMALLINT, 
	localeid SMALLINT, 
	type INT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid),
	FOREIGN KEY(unitid) REFERENCES unitnames(unitid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

CREATE TABLE IF NOT EXISTS productdescriptions 
(
	productid INT, 
	description TEXT, 
	isdefault CHAR(1), 
	type CHAR(1), 
	localeid SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

CREATE TABLE IF NOT EXISTS productlocales 
(
	productid INT, 
	isactive CHAR(1), 
	published TINYTEXT, 
	FOREIGN KEY(productid) REFERENCES product(productid)
);

CREATE TABLE IF NOT EXISTS productaccessories 
(
	productid INT, 
	accessoryid INT, 
	localeid SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

CREATE TABLE IF NOT EXISTS searchattributevalues
(
	valueid INT,
	value TEXT,
	absolutevalue DOUBLE,
	unitid INT,
	isabsolute SMALLINT,
	PRIMARY KEY(valueid),
	FOREIGN KEY(unitid) REFERENCES units(unitid)
);

CREATE TABLE IF NOT EXISTS searchattributes 
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

CREATE TABLE IF NOT EXISTS productkeywords 
(
	productid INT, 
	text TEXT, 
	localeid SMALLINT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

CREATE TABLE IF NOT EXISTS categorysearchattributes
(
	categoryid INT,
	attributeid BIGINT,
	isactive SMALLINT,
	FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid)
);

CREATE TABLE IF NOT EXISTS categoryheader
(
	headerid INT,
	categoryid INT,
	isactive SMALLINT,
	templatetype SMALLINT,
	displayorder SMALLINT,
	FOREIGN KEY(headerid) REFERENCES headernames(headerid),
	FOREIGN KEY(categoryid) REFERENCES category(categoryid)
);

CREATE TABLE IF NOT EXISTS categorynames
(
	categoryid INT,
	name TEXT,
	localeid SMALLINT,
	FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);

CREATE TABLE IF NOT EXISTS categorydisplayattributes 
(
	headerid INT,
	categoryid INT,
	attributeid BIGINT,
	isactive SMALLINT,
	templatetype SMALLINT,
	displayorder SMALLINT,
	FOREIGN KEY(headerid) REFERENCES headernames(headerid),
	FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid)
);