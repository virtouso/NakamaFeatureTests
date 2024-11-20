package leaderboard

import (
	"TrueSoldier3dMetaServer/consts"
	"TrueSoldier3dMetaServer/models/dto"
	"context"
	"database/sql"
	"encoding/json"
	"errors"
	"fmt"
	"github.com/heroiclabs/nakama-common/api"
	"github.com/heroiclabs/nakama-common/runtime"
)

func RegisterLeaderboardRelated(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {

	makeGeneral(ctx, logger, db, nk)
	makeWeekly(ctx, logger, db, nk)
	initializer.RegisterLeaderboardReset(resetLeaderboard)
	if err := initializer.RegisterRpc("custom_rpc_method", customRPC); err != nil {
		logger.Error("Unable to register RPC: %v", err)
		return err
	}

	if err := initializer.RegisterRpc("send_score", sendScore); err != nil {
		logger.Error("Unable to register RPC: %v", err)
		return err
	}

	logger.Info("RPC method 'custom_rpc_method' registered successfully")
	return nil
}

func customRPC(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	// Log and process the payload
	logger.Info("Received payload: %s", payload)
	response := fmt.Sprintf("Hello from Nakama! You sent: %s", payload)

	// Return the response
	return response, nil
}

func sendScore(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {

	processScore := func(win bool) int64 {
		if win {
			return 100
		}
		return 0
	}

	var request dto.MatchResult
	if err := json.Unmarshal([]byte(payload), &request); err != nil {
		return "", errors.New("invalid payload")
	}

	score := processScore(request.Win)

	general := "global"
	userId := ctx.Value(runtime.RUNTIME_CTX_USER_ID).(string)
	username := ctx.Value(runtime.RUNTIME_CTX_USERNAME).(string)

	storageData, _ := nk.StorageRead(ctx, []*runtime.StorageRead{{
		Collection: consts.Competition,
		Key:        consts.Leaderboard,
		UserID:     userId,
	}})

	leaderboardId := ""

	for _, i2 := range storageData {
		if i2.Key == consts.WeeklyName {
			leaderboardId = i2.Value
		}
	}

	nk.LeaderboardRecordWrite(ctx, leaderboardId, userId, username, score, 0, map[string]interface{}{}, nil)
	nk.LeaderboardRecordWrite(ctx, general, userId, username, score, 0, map[string]interface{}{}, nil)
	return "", nil
}

func makeGeneral(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule) {
	id := "global"
	authoritative := true
	sort := "desc"
	operator := "best"
	reset := ""
	metadata := map[string]interface{}{"test_key": "sample"}

	if err := nk.LeaderboardCreate(ctx, id, authoritative, sort, operator, reset, metadata, true); err != nil {
		logger.Error("Unable to create leaderboard: %v", err)
	}
}

func makeWeekly(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule) {
	id := "weekly"
	authoritative := true
	sort := "desc"
	operator := "best"
	reset := "0 0 * * 5"
	metadata := map[string]interface{}{"test_key": "sample"}

	ids := []string{"newbies_1", "newbies_2", "middle_1", "middle_2", "advanced_1", "advanced_2"}

	for _, s := range ids {

		if err := nk.LeaderboardCreate(ctx, id+"_"+s, authoritative, sort, operator, reset, metadata, true); err != nil {
			logger.Error("Unable to register Leaderboard: %v", err)
		}

	}

}

func resetLeaderboard(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, leaderboard *api.Leaderboard, reset int64) error {
	return nil
}
