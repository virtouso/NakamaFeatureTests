package match_making

import (
	backApi "TrueSoldier3dMetaServer/api"
	"TrueSoldier3dMetaServer/helper"
	"context"
	"database/sql"
	"fmt"

	"github.com/heroiclabs/nakama-common/runtime"
	"google.golang.org/protobuf/encoding/protojson"
)

type nakamaRpcFunc func(context.Context, runtime.Logger, *sql.DB, runtime.NakamaModule, string) (string, error)

func RpcFindMatch(marshaler *protojson.MarshalOptions, unmarshaler *protojson.UnmarshalOptions) nakamaRpcFunc {
	return func(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
		_, ok := ctx.Value(runtime.RUNTIME_CTX_USER_ID).(string)
		if !ok {
			return "", helper.ErrNoUserIdFound
		}

		request := &backApi.RpcFindMatchRequest{}

		if err := unmarshaler.Unmarshal([]byte(payload), request); err != nil {
			return "", helper.ErrUnmarshal
		}

		// If AI flag is set just create a brand-new match
		if request.Ai {
			matchID, err := nk.MatchCreate(
				ctx, moduleName, map[string]interface{}{
					"ai": true, "fast": request.Fast})
			if err != nil {
				logger.Error("error creating match: %v", err)
				return "", helper.ErrInternalError
			}

			response, err := marshaler.Marshal(&backApi.RpcFindMatchResponse{
				MatchIds: []string{matchID}})
			if err != nil {
				logger.Error("error marshaling response payload: %v", err.Error())
				return "", helper.ErrMarshal
			}

			logger.Info("new AI match created %s", matchID)

			return string(response), nil
		}

		maxSize := 1
		var fast int
		if request.Fast {
			fast = 1
		}
		query := fmt.Sprintf("+label.open:1 +label.fast:%d", fast)

		matchIDs := make([]string, 0, 10)
		matches, err := nk.MatchList(ctx, 10, true, "", nil, &maxSize, query)
		if err != nil {
			logger.Error("error listing matches: %v", err)
			return "", helper.ErrInternalError
		}
		if len(matches) > 0 {
			// There are one or more ongoing matches the user could join.
			for _, match := range matches {
				matchIDs = append(matchIDs, match.MatchId)
			}
		} else {
			// No available matches found, create a new one.
			matchID, err := nk.MatchCreate(ctx, moduleName, map[string]interface{}{"fast": request.Fast})
			if err != nil {
				logger.Error("error creating match: %v", err)
				return "", helper.ErrInternalError
			}
			matchIDs = append(matchIDs, matchID)
		}

		response, err := marshaler.Marshal(&backApi.RpcFindMatchResponse{MatchIds: matchIDs})
		if err != nil {
			logger.Error("error marshaling response payload: %v", err.Error())
			return "", helper.ErrMarshal
		}

		return string(response), nil
	}
}
