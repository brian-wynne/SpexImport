ALTER TABLE unitnames_temp
	ADD FOREIGN KEY(unitid) REFERENCES units(unitid);