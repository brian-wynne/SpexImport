ALTER TABLE productattributes_temp
	ADD FOREIGN KEY(productid) REFERENCES product(productid),
	ADD FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid),
	ADD FOREIGN KEY(unitid) REFERENCES unitnames(unitid),
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);