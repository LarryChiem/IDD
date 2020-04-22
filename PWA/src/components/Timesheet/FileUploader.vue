<template>
  <div class="example-drag">
    <div class="upload">
      <ul v-if="files.length">
        <li v-for="(file) in files" :key="file.id">
          <span data-testid="name">{{file.name}}</span> -
          <span>{{file.size | formatSize}}</span> -
          <span v-if="file.error">{{file.error}}</span>
          <span v-else-if="file.success">success</span>
          <span v-else-if="file.active">active</span>
          <span v-else></span>
        </li>
      </ul>
      <ul v-else>
        <td colspan="7">
          <div class="text-center p-5">
            <h4>Drop files anywhere to upload<br />or</h4>
            <label for="file" class="btn btn-lg btn-primary"
              >Select Files</label
            >
          </div>
        </td>
      </ul>

      <div v-show="$refs.upload && $refs.upload.dropActive" class="drop-active">
        <h3>Drop files to upload</h3>
      </div>

      <div class="example-btn">
        <file-upload
          class="btn btn-primary"
					:post-action= 'url'
          :multiple="true"
          :drop="true"
          :drop-directory="true"
          :maximum="2"
          :size="1024 * 1024 * 10"
          accept="image/*, application/pdf"
          v-model="files"
          ref="upload"
        >
          <i class="fa fa-plus"></i>
          Select files
        </file-upload>
        <button @click="types" type="button" class="btn btn-success" v-if="!$refs.upload || !$refs.upload.active" @click.prevent="$refs.upload.active = true">
          <i id="startUpload" class="fa fa-arrow-up" aria-hidden="true"></i>
          Start Upload
        </button>
        <button
          type="button"
          class="btn btn-danger"
          v-else
          @click.prevent="$refs.upload.active = false"
        >
          <i class="fa fa-stop" aria-hidden="true"></i>
          Stop Upload
        </button>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.example-drag.drop-space {
  padding-inline-start: 0;
  background: #000;
}
.example-drag.upload {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-flow: column;
  padding-top: 50em;
}
.example-drag {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-flow: column;
  padding-top: 5em;
}
.example-drag.btn {
  margin-bottom: 0;
  margin-right: 1rem;
}
.example-drag.drop-active {
  top: 5em;
  bottom: 0;
  right: 0;
  left: 0;
  position: fixed;
  z-index: 9999;
  opacity: 0.6;
  text-align: center;
  background: #000;
}
.example-drag.drop-active h3 {
  margin: -0.5em 0 0;
  position: absolute;
  top: 50em;
  left: 5em;
  right: 0;
  -webkit-transform: translateY(-50%);
  -ms-transform: translateY(-50%);
  transform: translateY(-50%);
  font-size: 40px;
  color: #fff;
  padding: 0;
}
</style>

<script>
import FileUpload from 'vue-upload-component'

export default {
  name: "file_uploader",
  components: {
    FileUpload
  },
	props: {
		uploadFiles: {
			type: Array,
			defaut: () => [],
			validator: value =>
			Array.isArray(value) && value.every(file => file.id && file.name && file.type)},
	},
	methods: {
		types() {
			this.uploadStatus = 1
			return 1
		}
	},
  data() {
    return {
      files: [],
			uploadStatus: 0,
			url: process.env.VUE_APP_SERVER_URL.concat('ImageUpload/DocAsForm'),

    }
  },
}

</script>
