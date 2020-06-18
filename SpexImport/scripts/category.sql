CREATE TABLE IF NOT EXISTS category_temp
(
	categoryid INT,
	parentcategoryid INT,
	isactive SMALLINT,
	ordernumber SMALLINT,
	catlevel SMALLINT,
	PRIMARY KEY(categoryid)
);

ALTER TABLE category_temp ADD FOREIGN KEY(parentcategoryid) REFERENCES category_temp(categoryid);