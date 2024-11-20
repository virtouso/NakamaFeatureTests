package database

type PlayerData struct {
	LeaderboardData PlayerLeaderboardData
}

type PlayerLeaderboardData struct {
	CurrentWeekly string
}
