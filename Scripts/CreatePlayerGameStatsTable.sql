CREATE TABLE `everybody_edits_ctf`.'player_game_stats'
(
    `PlayerId` INT NOT NULL,
    `TotalWins` INT NOT NULL DEFAULT '0',
    `TotalLosses` INT NOT NULL DEFAULT '0',
    `TotalKills` INT NOT NULL DEFAULT '0',
    `TotalCoins` INT NOT NULL DEFAULT '0',
    PRIMARY KEY (`PlayerId`)
) ENGINE = InnoDB;