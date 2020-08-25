CREATE TABLE `everybody_edits_ctf`.`players`
(
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Username` TEXT NOT NULL,
    `LastVisitDate` DATE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsAdministrator` BOOLEAN NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE = InnoDB;