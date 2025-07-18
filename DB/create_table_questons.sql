CREATE TABLE questions(
    id BIGSERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    question_text TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(user_id) REFERENCES users(id)
);

COMMENT ON TABLE questions IS '質問管理テーブル';
COMMENT ON COLUMN questions.id IS '質問ID';
COMMENT ON COLUMN questions.user_id IS '質問者のユーザID';
COMMENT ON COLUMN questions.question_text IS '質問内容';
COMMENT ON COLUMN questions.created_at IS '質問作成日時'