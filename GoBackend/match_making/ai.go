package match_making

import (
	backApi "TrueSoldier3dMetaServer/api"
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"math"
	"net/http"
	"time"

	"github.com/heroiclabs/nakama-common/runtime"
)

const aiUserId = "ai-user-id"

var aiPresenceObj = &aiPresence{}

var _ runtime.Presence = (*aiPresence)(nil)
var _ runtime.MatchData = (*aiMatchData)(nil)

type aiPresence struct{}
type aiMatchData struct {
	opCode backApi.OpCode
	data   []byte
	*aiPresence
}

func (ap *aiPresence) GetHidden() bool {
	return false
}

func (ap *aiPresence) GetPersistence() bool {
	return false
}

func (ap *aiPresence) GetUsername() string {
	return "ai-player"
}

func (ap *aiPresence) GetStatus() string {
	return ""
}

func (ap *aiPresence) GetReason() runtime.PresenceReason {
	return runtime.PresenceReasonUnknown
}

func (ap *aiPresence) GetUserId() string {
	return aiUserId
}

func (ap *aiPresence) GetSessionId() string {
	return ""
}

func (ap *aiPresence) GetNodeId() string {
	return ""
}

func (amd *aiMatchData) GetOpCode() int64 {
	return int64(amd.opCode)
}

func (amd *aiMatchData) GetData() []byte {
	return amd.data
}

func (amd *aiMatchData) GetReliable() bool {
	return true
}

func (amd *aiMatchData) GetReceiveTime() int64 {
	return time.Now().UTC().Unix()
}

type cell [2]int
type row [3]cell
type board [3]row

type tfRequest struct {
	Instances [1]board `json:"instances"`
}

type tfResponse struct {
	Predictions [][]float64 `json:"predictions"`
}

func (m *MatchHandler) aiTurn(s *MatchState) error {
	// Convert board state into expected model format
	b := board{}

	for i, mark := range s.board {
		rowIdx := i / 3
		cellIdx := i % 3

		switch mark {
		case s.marks[aiUserId]: // AI
			b[rowIdx][cellIdx] = cell{1, 0}
		case backApi.Mark_MARK_UNSPECIFIED:
			b[rowIdx][cellIdx] = cell{0, 0}
		default: // Player
			b[rowIdx][cellIdx] = cell{0, 1}
		}
	}

	// Send the vectors to TF
	req := tfRequest{Instances: [1]board{b}}
	raw, err := json.Marshal(req)
	if err != nil {
		return fmt.Errorf("failed to marshal TF request: %w", err)
	}

	resp, err := http.Post(
		m.TfServingAddress, "application/json", bytes.NewReader(raw))

	if err != nil {
		return fmt.Errorf("failed to make TF request: %w", err)
	}
	defer resp.Body.Close()

	respBody, err := io.ReadAll(resp.Body)
	if err != nil {
		return fmt.Errorf("failed to make TF request: %w", err)
	}

	// Convert response into message
	predictions := tfResponse{}
	if err := json.Unmarshal(respBody, &predictions); err != nil {
		return fmt.Errorf("failed to unmarshal TF response: %w", err)
	}

	if len(predictions.Predictions) != 1 {
		return fmt.Errorf("received unexpected TF response: %w", err)
	}

	// Find the index with the highest predicted value
	maxVal := math.Inf(-1)
	aiMovePos := -1
	for i, val := range predictions.Predictions[0] {
		if val > maxVal {
			maxVal = val
			aiMovePos = i
		}
	}

	// Append message to m.messages to be consumed by the next loop run
	if aiMovePos > -1 {
		move := &backApi.Move{Position: int32(aiMovePos)}
		rawMove, err := m.Marshaler.Marshal(move)
		if err != nil {
			return fmt.Errorf("failed to marshal AI move: %w", err)
		}

		data := &aiMatchData{
			opCode:     backApi.OpCode_OPCODE_MOVE,
			data:       rawMove,
			aiPresence: aiPresenceObj,
		}

		s.messages <- data
	}

	return nil
}
