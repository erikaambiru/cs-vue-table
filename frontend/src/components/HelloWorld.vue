<template>
  <div>
    <h2>商品一覧（複数選択可）</h2>
    <table border="1" style="margin:auto; min-width:320px;">
      <thead>
        <tr>
          <th><input type="checkbox" :checked="isAllSelected" @change="toggleAll" /></th>
          <th>ID</th>
          <th>商品名</th>
          <th>価格</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in items" :key="item.id">
          <td><input type="checkbox" :value="item.id" v-model="selected" /></td>
          <td>{{ item.id }}</td>
          <td>{{ item.name }}</td>
          <td>{{ item.price }}円</td>
        </tr>
      </tbody>
    </table>
    <div style="margin-top:20px;">
      <strong>選択中のID:</strong> {{ selected.join(', ') }}
    </div>
    <button @click="downloadSelected" :disabled="selected.length === 0" style="margin-top:10px;">選択したデータをJSONでダウンロード</button>
    <div v-if="error" style="color:red; margin-top:20px;">
      エラー: {{ error }}
    </div>
  </div>
</template>

<script>
import JSZip from 'jszip';
export default {
  name: 'ItemTable',
  data() {
    return {
      items: [],
      selected: [],
      error: ''
    };
  },
  computed: {
    isAllSelected() {
      return this.items.length > 0 && this.selected.length === this.items.length;
    }
  },
  methods: {
    toggleAll(event) {
      if (event.target.checked) {
        this.selected = this.items.map(item => item.id);
      } else {
        this.selected = [];
      }
    },
    async downloadSelected() {
      const selectedItems = this.items.filter(item => this.selected.includes(item.id));
      if (selectedItems.length === 0) return;
      const zip = new JSZip();
      for (const item of selectedItems) {
        const json = JSON.stringify(item, null, 2);
        zip.file(`item_${item.id}.json`, json);
      }
      const content = await zip.generateAsync({ type: 'blob' });
      const url = URL.createObjectURL(content);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'selected_items.zip';
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }
  },
  mounted() {
    fetch('/api/items')
      .then(res => {
        if (!res.ok) throw new Error('API取得失敗: ' + res.status);
        return res.json();
      })
      .then(data => {
        this.items = data;
      })
      .catch(e => {
        this.error = e.message;
      });
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
