CREATE TABLE IF NOT EXISTS categoryheader_temp
(
	headerid INT,
	categoryid INT,
	isactive SMALLINT,
	templatetype SMALLINT,
	displayorder SMALLINT,
	FOREIGN KEY(headerid) REFERENCES headernames(headerid),
	FOREIGN KEY(categoryid) REFERENCES category(categoryid)
);