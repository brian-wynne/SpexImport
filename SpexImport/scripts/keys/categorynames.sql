ALTER TABLE categorynames_temp
	ADD FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);
