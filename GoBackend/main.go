package main

import (
	"TrueSoldier3dMetaServer/leaderboard"
	"context"
	"database/sql"
	"github.com/heroiclabs/nakama-common/runtime"
)

const (
	rpcIdRewards   = "rewards"
	rpcIdFindMatch = "find_match"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	//err := guards.RegisterAllGuards(initializer)
	//if err != nil {
	//	return err
	//}

	//initStart := time.Now()
	//
	//marshaler := &protojson.MarshalOptions{
	//	UseEnumNumbers: true,
	//}
	//unmarshaler := &protojson.UnmarshalOptions{
	//	DiscardUnknown: false,
	//}
	//
	//if err := initializer.RegisterRpc(rpcIdRewards, reward.RpcRewards); err != nil {
	//	return err
	//}
	//
	//if err := initializer.RegisterRpc(rpcIdFindMatch, match_making.RpcFindMatch(marshaler, unmarshaler)); err != nil {
	//	return err
	//}
	//
	//if err := initializer.RegisterMatch(match_making.ModuleName, func(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule) (runtime.Match, error) {
	//	return &match_making.MatchHandler{
	//		Marshaler:        marshaler,
	//		Unmarshaler:      unmarshaler,
	//		TfServingAddress: "http://tf:8501/v1/models/ttt:predict",
	//	}, nil
	//}); err != nil {
	//	return err
	//}
	//
	//if err := session_management.RegisterSessionEvents(db, nk, initializer); err != nil {
	//	return err
	//}
	//
	//logger.Info("Plugin loaded in '%d' msec.", time.Now().Sub(initStart).Milliseconds())
	//

	err := leaderboard.RegisterLeaderboardRelated(ctx, logger, db, nk, initializer)
	if err != nil {
		return err
	}

	return nil
}
