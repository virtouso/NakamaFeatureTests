package main

import (
	"context"
	"database/sql"
	"github.com/heroiclabs/nakama-common/runtime"
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	// Register the RPC function.
	if err := initializer.RegisterRpc("custom_endpoint", customEndpointHandler); err != nil {
		return err
	}
	return nil
}

func customEndpointHandler(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	// Handle the request and prepare a response.
	response := map[string]string{
		"message": "changiz",
	}

	// Convert response to JSON (if needed, add json.Marshal).
	return response["message"], nil
}
