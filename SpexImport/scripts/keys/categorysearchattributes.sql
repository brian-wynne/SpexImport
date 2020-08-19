ALTER TABLE categorysearchattributes_temp
	ADD FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	ADD FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid);