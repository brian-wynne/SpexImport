ALTER TABLE productlocales_temp
	ADD FOREIGN KEY(productid) REFERENCES product(productid);